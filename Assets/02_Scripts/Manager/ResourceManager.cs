using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager
{
    private Dictionary<string, AsyncOperationHandle> _assetHandles = new();
    private Dictionary<string, UniTaskCompletionSource<UnityEngine.Object>> _currentLoading = new();

    private CancellationTokenSource _allReleaseToken = new();

    private const int MAX_LOAD_COUNT = 4;


    public async UniTask PreloadAssetsAsync(Action<float> onProgress = null)
    {
        int progressCount = 0;
        onProgress?.Invoke(0f);

        var dataTable = GameManager.DataTable.PreLoadAssetDataTable;

        int totalCount = dataTable.Count;

        if (dataTable.Count == 0)
        {
            onProgress?.Invoke(1f);
            return;
        }

        List<UniTask> loadTasks = new(totalCount);
        using SemaphoreSlim semaphore = new(MAX_LOAD_COUNT);

        foreach (PreLoadAssetData preLoadData in dataTable.Values)
        {
            loadTasks.Add(LoadWithSemaphoreAsync(preLoadData, semaphore,
                () =>
                {
                    progressCount++;
                    onProgress?.Invoke(progressCount / (float)totalCount);
                }));
        }

        await UniTask.WhenAll(loadTasks);

        onProgress?.Invoke(1f);
    }

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
    }

    private async UniTask LoadWithSemaphoreAsync(PreLoadAssetData preLoadData, SemaphoreSlim semaphore, Action onCompleted)
    {
        await semaphore.WaitAsync();

        try
        {
            switch (preLoadData.AssetType)
            {
                case "Mesh":
                    await LoadAssetAsync<Mesh>(preLoadData.Address);
                    break;

                case "Material":
                    await LoadAssetAsync<Material>(preLoadData.Address);
                    break;

                case "Prefab":
                case "GameObject":
                    await LoadAssetAsync<GameObject>(preLoadData.Address);
                    break;

                case "AudioClip":
                    await LoadAssetAsync<AudioClip>(preLoadData.Address);
                    break;

                default:
                    await LoadAssetAsync<UnityEngine.Object>(preLoadData.Address);
                    break;
            }
        }
        finally
        {
            onCompleted?.Invoke();
            semaphore.Release();
        }
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
}
