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
    private Dictionary<string, UnityEngine.Object> _loadedAssets = new();
    private Dictionary<string, List<AsyncOperationHandle>> _assetHandles = new();
    private Dictionary<string, HashSet<string>> _contentAddresses = new();

    private const int _maxLoadCount = 4;

    public T GetLoadedAsset<T>(string address) where T : UnityEngine.Object
    {
        if (!_loadedAssets.TryGetValue(address, out UnityEngine.Object asset))
        {
            Logger.LogError($"{address}는 로드되지 않은 에셋입니다.");
            return null;
        }

        if (asset is not T castedAsset)
        {
            Logger.LogError($"{address}는 {typeof(T).Name} 타입이 아닙니다. 실제 타입: {asset.GetType().Name}");
            return null;
        }

        return castedAsset;
    }

    public async UniTask LoadContentAsync(string label, Action<float> onProgress = null)
    {
        if (Utils.IsNullOrWhiteSpace(label))
        {
            Logger.LogError($"콘텐츠 라벨({nameof(label)})이 비어 있습니다.");
            return;
        }

        AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = default;
        AsyncOperationHandle<IList<IResourceLocation>> spriteLocationHandle = default;

        try
        {
            onProgress?.Invoke(0f);

            locationsHandle = Addressables.LoadResourceLocationsAsync(label, typeof(UnityEngine.Object));
            spriteLocationHandle = Addressables.LoadResourceLocationsAsync(label, typeof(Sprite));

            IList<IResourceLocation> locations = await locationsHandle.ToUniTask();
            IList<IResourceLocation> spriteLocations = await spriteLocationHandle.ToUniTask();

            if (locations.Count == 0 && spriteLocations.Count == 0)
            {
                Logger.LogWarning($"{label} 라벨에 등록된 에셋이 없습니다.");
                return;
            }

            Dictionary<string, IResourceLocation> assetLocations = new();
            Dictionary<string, IResourceLocation> spriteAssetLocations = new();

            foreach (IResourceLocation location in locations)
            {
                string address = location.PrimaryKey;

                if (!assetLocations.ContainsKey(address))
                {
                    assetLocations.Add(address, location);
                }
            }

            foreach (IResourceLocation location in spriteLocations)
            {
                string address = location.PrimaryKey;

                if (!spriteAssetLocations.ContainsKey(address))
                {
                    spriteAssetLocations.Add(address, location);
                }
            }

            foreach (string spriteAddress in spriteAssetLocations.Keys)
            {
                assetLocations.Remove(spriteAddress);
            }

            int totalCount = assetLocations.Count + spriteAssetLocations.Count;

            _assetHandles.Add(label, new List<AsyncOperationHandle>());
            _contentAddresses[label] = new HashSet<string>();

            List<UniTask> loadTasks = new(totalCount);
            int loadedCount = 0;

            using SemaphoreSlim semaphore = new(_maxLoadCount);

            Action onCompleted = () =>
            {
                loadedCount++;
                onProgress?.Invoke(loadedCount / (float)totalCount);
            };

            foreach (KeyValuePair<string, IResourceLocation> pair in spriteAssetLocations)
            {
                loadTasks.Add(LoadSpriteContentAsync(label, pair.Value, semaphore, onCompleted));
            }

            foreach (KeyValuePair<string, IResourceLocation> pair in assetLocations)
            {
                loadTasks.Add(LoadContentAssetAsync(label, pair.Value, semaphore, onCompleted));
            }

            await UniTask.WhenAll(loadTasks);

            onProgress?.Invoke(1f);
        }
        finally
        {
            if (locationsHandle.IsValid())
            {
                Addressables.Release(locationsHandle);
            }

            if (spriteLocationHandle.IsValid())
            {
                Addressables.Release(spriteLocationHandle);
            }
        }
    }

    public void ReleaseContent(string label)
    {
        _contentAddresses.Remove(label, out HashSet<string> addresses);
        _assetHandles.Remove(label, out List<AsyncOperationHandle> handles);

        if (addresses != null)
        {
            foreach (string address in addresses)
            {
                _loadedAssets.Remove(address);
            }
        }

        if (handles != null)
        {
            foreach (AsyncOperationHandle handle in handles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }
    }

    public void ReleaseAllAssets()
    {
        foreach (List<AsyncOperationHandle> handles in _assetHandles.Values)
        {
            foreach (AsyncOperationHandle handle in handles)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
            }
        }

        _loadedAssets.Clear();
        _assetHandles.Clear();
        _contentAddresses.Clear();
    }

    public async UniTask LoadAllLabelAssetAsync(Action<float> onProgress)
    {
        string[] labels =
        {
            AddressablePath.Label.COMMON,
            AddressablePath.Label.REALITY,
            AddressablePath.Label.DREAM
        };

        for (int i = 0; i < labels.Length; i++)
        {
            int labelIndex = i;

            await LoadContentAsync(labels[labelIndex],
                progress =>
                {
                    float totalProgress = (labelIndex + progress) / labels.Length;

                    onProgress?.Invoke(totalProgress);
                });
        }
    }

    private async UniTask LoadContentAssetAsync(string label, IResourceLocation location, SemaphoreSlim semaphore, Action onCompleted)
    {
        await semaphore.WaitAsync();

        try
        {
            UnityEngine.Object asset = await LoadAssetAsync(location, label);
            string address = location.PrimaryKey;

            _loadedAssets[address] = asset;
            _contentAddresses[label].Add(address);
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"콘텐츠 에셋 로드 실패 - Label: {label}, Address: {location.PrimaryKey}, Exception: {exception.Message}");
        }
        finally
        {
            onCompleted?.Invoke();
            semaphore.Release();
        }
    }

    private async UniTask<UnityEngine.Object> LoadAssetAsync(IResourceLocation location, string label)
    {
        AsyncOperationHandle<UnityEngine.Object> handle = default;

        try
        {
            handle = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
            UnityEngine.Object asset = await handle.ToUniTask();

            if (asset == null)
            {
                Logger.LogError($"{location.PrimaryKey}가 null입니다.");
            }

            _assetHandles[label].Add(handle);
            return asset;
        }
        catch
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            throw;
        }
    }

    private async UniTask LoadSpriteContentAsync(string label, IResourceLocation location, SemaphoreSlim semaphore, Action onCompleted)
    {
        await semaphore.WaitAsync();

        string address = location.PrimaryKey;

        try
        {
            IList<Sprite> sprites = await LoadSpriteAssetsAsync(address, label);

            foreach (Sprite sprite in sprites)
            {
                string spriteAddress = $"{address}[{sprite.name}]";

                _loadedAssets[spriteAddress] = sprite;
                _contentAddresses[label].Add(spriteAddress);
            }

            if (sprites.Count == 1)
            {
                _loadedAssets[address] = sprites[0];
                _contentAddresses[label].Add(address);

            }
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"Sprite 콘텐츠 로드 실패 - Label: {label}, Address: {address}, Exception: {exception.Message}");
        }
        finally
        {
            onCompleted?.Invoke();
            semaphore.Release();
        }
    }

    private async UniTask<IList<Sprite>> LoadSpriteAssetsAsync(string address, string label)
    {
        AsyncOperationHandle<IList<Sprite>> handle = default;

        try
        {
            handle = Addressables.LoadAssetAsync<IList<Sprite>>(address);
            IList<Sprite> sprites = await handle.ToUniTask();

            if (sprites == null || sprites.Count == 0)
            {
                Logger.LogError($"{address}에 등록된 Sprite가 없습니다.");
            }

            _assetHandles[label].Add(handle);
            return sprites;
        }
        catch
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }

            throw;
        }
    }

}
