using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager
{
    private Dictionary<string, Queue<GameObject>> _objectPools = new();
    private Dictionary<string, List<GameObject>> _activeObjects = new();
    private Dictionary<string, GameObject> _prefabs = new();
    private Transform _poolRoot;

    public async UniTask InitAsync(Transform poolRoot)
    {
        _poolRoot = poolRoot;
        var poolDataTable = GameManager.DataTable.PoolDataTable;

        foreach (string poolId in poolDataTable.Keys)
        {
            _objectPools.Add(poolId, new Queue<GameObject>());

            GameObject prefab = await LoadGameObjectAsync(poolId);
            _prefabs.Add(poolId, prefab);

            for (int i = 0; i < poolDataTable[poolId].InitSize; i++)
            {
                CreateNewObject(poolId, prefab);
            }
        }
    }

    public GameObject SpawnFromPool(string poolId)
        => GetFromPool(poolId, Vector3.zero, Quaternion.identity);
    public GameObject SpawnFromPool(string poolId, Vector3 position)
        => GetFromPool(poolId, position, Quaternion.identity);
    public GameObject SpawnFromPool(string poolId, Vector3 position, Quaternion rotation)
        => GetFromPool(poolId, position, rotation);
    public GameObject SpawnFromPool(string poolId, Vector3 position, Quaternion rotation, Transform parent)
        => GetFromPool(poolId, position, rotation, parent);
    public GameObject SpawnFromPool(string poolId, Transform parent, bool usePrefabTransform)
        => GetFromPool(poolId, Vector3.zero, Quaternion.identity, parent, usePrefabTransform);

    public T SpawnFromPool<T>(string poolId) where T : Component
    {
        GameObject obj = GetFromPool(poolId, Vector3.zero, Quaternion.identity);
        if (obj.TryGetComponent<T>(out T component))
            return component;
        else
            throw new Exception($"Pool Id({poolId})에 올바른 컴포넌트가 존재하지 않습니다. component: {typeof(T)}.");
    }

    public T SpawnFromPool<T>(string poolId, Vector3 position) where T : Component
    {
        GameObject obj = GetFromPool(poolId, position, Quaternion.identity);
        if (obj.TryGetComponent<T>(out T component))
            return component;
        else
            throw new Exception($"Pool Id({poolId})에 올바른 컴포넌트가 존재하지 않습니다. component: {typeof(T)}.");
    }
    public T SpawnFromPool<T>(string poolId, Vector3 position, Quaternion rotation) where T : Component
    {
        GameObject obj = GetFromPool(poolId, position, rotation);
        if (obj.TryGetComponent<T>(out T component))
            return component;
        else
            throw new Exception($"Pool Id({poolId})에 올바른 컴포넌트가 존재하지 않습니다. component: {typeof(T)}.");
    }
    public T SpawnFromPool<T>(string poolId, Vector3 position, Quaternion rotation, Transform parent) where T : Component
    {
        GameObject obj = GetFromPool(poolId, position, rotation, parent);
        if (obj.TryGetComponent<T>(out T component))
            return component;
        else
            throw new Exception($"Pool Id({poolId})에 올바른 컴포넌트가 존재하지 않습니다. component: {typeof(T)}.");
    }
    public T SpawnFromPool<T>(string poolId, Transform parent, bool usePrefabTransform) where T : Component
    {
        GameObject obj = GetFromPool(poolId, Vector3.zero, Quaternion.identity, parent, usePrefabTransform);
        if (obj.TryGetComponent<T>(out T component))
            return component;
        else
            throw new Exception($"Pool Id({poolId})에 올바른 컴포넌트가 존재하지 않습니다. component: {typeof(T)}.");
    }

    public void DespawnToPool(GameObject obj)
    {
        if (obj == null)
            return;

        if (!_objectPools.ContainsKey(obj.name))
        {
            Logger.LogError($"Pool Id({obj.name})에 해당하는 풀이 없습니다.");
            return;
        }

        if (!_activeObjects.TryGetValue(obj.name, out var activeList) || !activeList.Remove(obj))
        {
            Logger.LogWarning($"Pool Id({obj.name}) 오브젝트는 이미 반환되었거나 활성 목록에 없습니다.");
            return;
        }

        _objectPools[obj.name].Enqueue(obj);

        obj.SetActive(false);
        obj.transform.SetParent(_poolRoot, false);
    }

    public void AllDespawnToPool()
    {
        foreach (var activedList in _activeObjects.Values)
        {
            for (int i = activedList.Count - 1; i >= 0; i--)
            {
                DespawnToPool(activedList[i]);
            }
        }
    }

    private GameObject GetFromPool(string poolId, Vector3 position, Quaternion rotation, Transform parent = null, bool usePrefabTransform = false)
    {
        if (!_objectPools.ContainsKey(poolId))
            throw new Exception($"Pool Id({poolId})에 해당하는 풀은 존재하지 않습니다.");

        Queue<GameObject> poolQueue = _objectPools[poolId];
        if (poolQueue.Count <= 0)
        {
            CreateNewObject(poolId, _prefabs[poolId]);
        }

        GameObject obj = poolQueue.Dequeue();

        if (null != parent)
        {
            obj.transform.SetParent(parent, false);
        }

        if (!usePrefabTransform)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }

        if (!_activeObjects.ContainsKey(poolId))
        {
            _activeObjects.Add(poolId, new());
        }

        _activeObjects[poolId].Add(obj);

        obj.gameObject.SetActive(true);

        return obj;
    }

    private GameObject CreateNewObject(string poolId, GameObject gameObject)
    {
        var obj = GameObject.Instantiate(gameObject, _poolRoot);
        obj.name = poolId;
        _objectPools[poolId].Enqueue(obj);
        obj.SetActive(false);

        return obj;
    }

    private async UniTask<GameObject> LoadGameObjectAsync(string address)
    {
        try
        {
            GameObject gameObject = await GameManager.Resource.LoadAssetAsync<GameObject>(address);
            return gameObject;
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"어드레서블에 등록되지 않은 에셋으로, 리소시즈에서 로드합니다. {exception.Message}");
        }

        GameObject resource = Resources.Load<GameObject>($"Pool/{address}");

        if (resource == null)
            throw new Exception($"({address}) 에셋을 Addressables와 Resources/Pool에서 찾을 수 없습니다.");

        return resource;
    }
}
