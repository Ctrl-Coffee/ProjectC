using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class ResourceManager
{
    private Dictionary<string, AsyncOperationHandle> _assetHandles = new();
    private Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _currentLoading = new();
    private Dictionary<string, HashSet<string>> _contentAddresses = new();

    private CancellationTokenSource _allReleaseToken = new();

    private const int _maxLoadCount = 4;

    public T GetLoadedAsset<T>(string address) where T: UnityEngine.Object
    {
        if (!_assetHandles.TryGetValue(address, out var handle))
        {
            Logger.LogError($"{address}는 로드되지 않은 에셋입니다.");
            return null;
        }

        return GetAssetFromHandle<T>(address, handle);
    }

    public async UniTask<T> LoadAssetAsync<T>(string address, CancellationToken cancelToken = default) where T : UnityEngine.Object
    {
        if (Utils.IsNullOrWhiteSpace(address))
            return null;

        if (_assetHandles.TryGetValue(address, out var handle))
        {
            T loadedAsset = GetAssetFromHandle<T>(address, handle);

            if (loadedAsset != null)
            {
                return loadedAsset;
            }

            Logger.LogWarning($"handle 이상: address {address}");
            _assetHandles.Remove(address);

            if(handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        if (_currentLoading.TryGetValue(address, out var loadingTask))
        {
            var loadingAsset = await loadingTask.Task.AttachExternalCancellation(cancellationToken: cancelToken);

            return loadingAsset as T;
        }

        var newTask = new UniTaskCompletionSource<UnityEngine.Object>();
        _currentLoading[address] = newTask;

        CancellationToken allReleaseToken = _allReleaseToken.Token;

        TryLoadAddressablesAssetAsync<T>(address, newTask, allReleaseToken).Forget();

        UnityEngine.Object asset = await newTask.Task.AttachExternalCancellation(cancelToken);

        return asset as T;
    }

    public async UniTask LoadContentAsync(string label, Action<float> onProgress = null,
        CancellationToken cancelToken = default)
    {
        if (Utils.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("콘텐츠 라벨이 비어 있습니다.", nameof(label));
        }

        if (_contentAddresses.ContainsKey(label))
        {
            onProgress?.Invoke(1f);
            return;
        }

        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = default;

        try
        {
            onProgress?.Invoke(0f);

            locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(UnityEngine.Object));
            IList<IResourceLocation> locations = await locationsHandle.ToUniTask(cancellationToken: cancelToken);

            if (locations.Count == 0)
            {
                throw new Exception($"{label} 라벨에 등록된 에셋이 없습니다.");
            }

            HashSet<string> addresses = new();
            List<UniTask> loadTasks = new(locations.Count);
            int loadedCount = 0;

            using SemaphoreSlim semaphore = new(_maxLoadCount);

            foreach (IResourceLocation location in locations)
            {
                string address = location.PrimaryKey;
                addresses.Add(address);

                loadTasks.Add(LoadContentAssetAsync(address, semaphore, cancelToken,
                    () =>
                    {
                        loadedCount++;
                        onProgress?.Invoke(loadedCount / (float)locations.Count);
                    }));
            }

            await UniTask.WhenAll(loadTasks);

            _contentAddresses[label] = addresses;
            onProgress?.Invoke(1f);
        }
        finally
        {
            if (locationsHandle.IsValid())
            {
                Addressables.Release(locationsHandle);
            }
        }
    }

    public bool IsContentLoaded(string label)
    {
        return _contentAddresses.ContainsKey(label);
    }

    public void ReleaseContent(string label)
    {
        if (!_contentAddresses.Remove(label, out HashSet<string> addresses))
        {
            return;
        }

        foreach (string address in addresses)
        {
            ReleaseAsset(address);
        }
    }

    public void ReleaseAsset(string address)
    {
        if (!_assetHandles.Remove(address, out var handle))
        {
            return;
        }

        if (handle.IsValid())
        {
            Addressables.Release(handle);
        }
    }

    public void ReleaseAllAssets()
    {
        _allReleaseToken.Cancel();
        _allReleaseToken.Dispose();

        _allReleaseToken = new CancellationTokenSource();

        foreach (var handle in _assetHandles.Values)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }

        _assetHandles.Clear();
        _currentLoading.Clear();
        _contentAddresses.Clear();
    }

    private T GetAssetFromHandle<T>(string address, AsyncOperationHandle handle) where T : UnityEngine.Object
    {
        if (!handle.IsValid())
        {
            Logger.LogError($"핸들 유효하지 않습니다. - Address: {address}");
            return null;
        }

        if (!handle.IsDone || handle.Status != AsyncOperationStatus.Succeeded)
        {
            Logger.LogError($"에셋 로드 못했습니다. - Address: {address}");
            return null;
        }

        T asset = handle.Result as T;

        if (asset == null)
        {
            Logger.LogError($"로드된 에셋이 null입니다. - Address: {address}");
            return null;
        }

        return asset;
    }

    private async UniTask TryLoadAddressablesAssetAsync<T>(string address, UniTaskCompletionSource<UnityEngine.Object> task
        , CancellationToken allReleaseToken) where T : UnityEngine.Object
    {
        AsyncOperationHandle<T> handle = default;

        try
        {
            handle = Addressables.LoadAssetAsync<T>(address);

            T asset = await handle.ToUniTask(cancellationToken: allReleaseToken);

            if (asset == null)
            {
                throw new Exception($"로드된 에셋이 null입니다. Address: {address}");
            }

            _assetHandles[address] = handle;

            task.TrySetResult(asset);
        }
        catch (OperationCanceledException)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            task.TrySetCanceled(allReleaseToken);
        }
        catch (Exception exception)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            task.TrySetException(exception);
        }
        finally
        {
            if (_currentLoading.TryGetValue(address, out var currentTask) && ReferenceEquals(currentTask, task))
            {
                _currentLoading.Remove(address);
            }
        }
    }

    private async UniTask LoadContentAssetAsync(string address, SemaphoreSlim semaphore,
        CancellationToken cancelToken, Action onCompleted)
    {
        await semaphore.WaitAsync(cancelToken);

        try
        {
            await LoadAssetAsync<UnityEngine.Object>(address, cancelToken);
        }
        finally
        {
            onCompleted?.Invoke();
            semaphore.Release();
        }
    }
}
