using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataTableManager
{
    #region Variables
    public Dictionary<string, PoolData> PoolDataTable { get; private set; } = new();
    public Dictionary<string, PreLoadAssetData> PreLoadAssetDataTable { get; private set; } = new();
    public Dictionary<string, AutoWorkData> AutoWorkDataTable { get; private set; } = new();


    #endregion

    public void LoadAllData()
    {
        // TODO: 데이터 테이블 만들기
        //PoolDataTable = LoadData<PoolData>(nameof(PoolData));
        //PreLoadAssetDataTable = LoadData<PreLoadAssetData>(nameof(PreLoadAssetData));

        // TODO: JSON 테이블이 생기면 LoadData<AutoWorkData>로 교체
        LoadDummyAutoWorkData();
    }

    #region Getters

    public PreLoadAssetData GetPreLoadAssetData(string id)
    {
        if (null == PreLoadAssetDataTable || string.IsNullOrEmpty(id)) return null;
        return PreLoadAssetDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public AutoWorkData GetAutoWorkData(string id)
    {
        if (null == AutoWorkDataTable || string.IsNullOrEmpty(id)) return null;
        return AutoWorkDataTable.TryGetValue(id, out var data) ? data : null;
    }

    #endregion

    #region Dummy

    private void LoadDummyAutoWorkData()
    {
        AutoWorkDataTable = new Dictionary<string, AutoWorkData>();

        AddDummyAutoWork("AutoWork_01", "aaa", 1800f, 100, 5);
        AddDummyAutoWork("AutoWork_02", "bbb", 3600f, 250, 12);
        AddDummyAutoWork("AutoWork_03", "ccc", 5400f, 420, 20);
        AddDummyAutoWork("AutoWork_04", "ddd", 7200f, 600, 30);
    }

    private void AddDummyAutoWork(string id, string name, float durationSeconds, int rewardMoney, int rewardDP)
    {
        AutoWorkDataTable.Add(id, new AutoWorkData
        {
            Id = id,
            Name = name,
            DurationSeconds = durationSeconds,
            RewardMoney = rewardMoney,
            RewardDP = rewardDP,
        });
    }

    #endregion

    [Serializable]
    class SerializationWrapper<T>
    {
        public List<T> items;
    }

    Dictionary<string, T> LoadData<T>(string tableNmae) where T : BaseData
    {
        string resourcePath = $"JsonOutput/{tableNmae}";
        TextAsset textAsset = Utils.ResourcesLoad<TextAsset>(resourcePath);
        if (null == textAsset)
        {
            Debug.LogError($"리소스를 찾을 수 없습니다: Resources/{resourcePath}");
            return new Dictionary<string, T>();
        }

        try
        {
            string jsonString = textAsset.text;

            string wrappedJson = "{\"items\":" + jsonString + "}";

            SerializationWrapper<T> wrapper = JsonUtility.FromJson<SerializationWrapper<T>>(wrappedJson);

            if (wrapper == null || wrapper.items == null)
            {
                Debug.LogError($"[{typeof(T).Name}] JSON 파싱 결과가 비어 있습니다.");
            }

            if (null != wrapper && null != wrapper.items)
            {
                Debug.Log($"{typeof(T).Name} 데이터를 {wrapper.items.Count}개 로드했습니다.");
                return wrapper.items.ToDictionary(value => value.Id.ToString());
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[{typeof(T).Name} JSON 로드 오류] {ex.Message}");
        }

        return new Dictionary<string, T>();
    }
}
