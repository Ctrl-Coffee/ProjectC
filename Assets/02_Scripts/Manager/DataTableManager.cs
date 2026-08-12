using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataTableManager
{
    #region Variables
    public Dictionary<string, PoolData> PoolDataTable { get; private set; } = new();
    public Dictionary<string, PreLoadAssetData> PreLoadAssetDataTable { get; private set; } = new();
    public Dictionary<string, WorkData> WorkDataTable { get; private set; } = new();

    public Dictionary<string, CompanionData> CompanionDataTable { get; private set; } = new();
    public Dictionary<string, CompanionLevelUpCostData> CompanionLevelUpCostDataTable { get; private set; } = new();
    public Dictionary<string, SkillData> SkillDataTable { get; private set; } = new();



    #endregion

    public void LoadAllData()
    {
        // TODO: 데이터 테이블 만들기
        //PoolDataTable = LoadData<PoolData>(nameof(PoolData));
        //PreLoadAssetDataTable = LoadData<PreLoadAssetData>(nameof(PreLoadAssetData));

        WorkDataTable = LoadData<WorkData>(nameof(WorkData));
        CompanionDataTable = LoadData<CompanionData>(nameof(CompanionData));
        CompanionLevelUpCostDataTable = LoadData<CompanionLevelUpCostData>(nameof(CompanionLevelUpCostData));
        SkillDataTable = LoadData<SkillData>(nameof(SkillData));
    }

    #region Getters

    public PreLoadAssetData GetPreLoadAssetData(string id)
    {
        if (null == PreLoadAssetDataTable || string.IsNullOrEmpty(id)) return null;
        return PreLoadAssetDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public WorkData GetWorkData(string id)
    {
        if (null == WorkDataTable || string.IsNullOrEmpty(id)) return null;
        return WorkDataTable.TryGetValue(id, out var data) ? data : null;
    }
    public CompanionData GetCompanionData(string id)
    {
        if (null == CompanionDataTable || string.IsNullOrEmpty(id)) return null;
        return CompanionDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public CompanionLevelUpCostData GetCompanionLevelUpCost(string id)
    {
        if (null == CompanionLevelUpCostDataTable || string.IsNullOrEmpty(id)) return null;
        return CompanionLevelUpCostDataTable.TryGetValue(id, out var data) ? data : null;
    }
    public SkillData GetSkillData(string id)
    {
        if (null == SkillDataTable || string.IsNullOrEmpty(id)) return null;
        return SkillDataTable.TryGetValue(id, out var data) ? data : null;
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
