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
    public Dictionary<string, ConfirmData> ConfirmDataTable { get; private set; } = new();

    public Dictionary<string, CompanionData> CompanionDataTable { get; private set; } = new();
    public Dictionary<string, CompanionLevelData> CompanionLevelDataTable { get; private set; } = new();
    public Dictionary<string, SkillData> SkillDataTable { get; private set; } = new();
    public Dictionary<string, PlayerLevelData> PlayerLevelDataTable { get; private set; } = new();
    public Dictionary<string, EquipmentLevelData> EquipmentLevelDataTable { get; private set; } = new();
    public Dictionary<string, EquipmentData> EquipmentDataTable { get; private set; } = new();
    public Dictionary<string, GachaProbabilityData> GachaProbabilityDataTable { get; private set; } = new();
    public Dictionary<string, PerkNodeData> PerkNodeDataTable { get; private set; } = new();
    public Dictionary<string, PerkEffectData> PerkEffectDataTable { get; private set; } = new();
    public Dictionary<string, WorkStatData> WorkStatDataTable { get; private set; } = new();

    private Dictionary<int, List<CompanionData>> _companionsByGrade = new();

    private Dictionary<GachaType, List<GachaProbabilityData>> _probabilitiesByGachaType = new();
    
    private Dictionary<int, List<EquipmentData>> _equipmentsByGrade = new();

    #endregion

    public void LoadAllData()
    {
        // TODO: 데이터 테이블 만들기
        //PoolDataTable = LoadData<PoolData>(nameof(PoolData));
        //PreLoadAssetDataTable = LoadData<PreLoadAssetData>(nameof(PreLoadAssetData));
        ConfirmDataTable = LoadData<ConfirmData>(nameof(ConfirmData));

        WorkDataTable = LoadData<WorkData>(nameof(WorkData));
        CompanionDataTable = LoadData<CompanionData>(nameof(CompanionData));
        CompanionLevelDataTable = LoadData<CompanionLevelData>(nameof(CompanionLevelData));
        SkillDataTable = LoadData<SkillData>(nameof(SkillData));
        PlayerLevelDataTable = LoadData<PlayerLevelData>(nameof(PlayerLevelData));
        EquipmentLevelDataTable = LoadData<EquipmentLevelData>(nameof(EquipmentLevelData));
        EquipmentDataTable = LoadData<EquipmentData>(nameof(EquipmentData));
        GachaProbabilityDataTable = LoadData<GachaProbabilityData>(nameof(GachaProbabilityData));
        PerkNodeDataTable = LoadData<PerkNodeData>(nameof(PerkNodeData));
        PerkEffectDataTable = LoadData<PerkEffectData>(nameof(PerkEffectData));
        WorkStatDataTable = LoadData<WorkStatData>(nameof(WorkStatData));

        BuildCompanionGradeIndex();
        BuildGachaProbabilityIndex();
        BuildEquipmentGradeIndex();
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

    public CompanionLevelData GetCompanionLevelData(string companionId, int level)
    {
        if (null == CompanionLevelDataTable || string.IsNullOrEmpty(companionId)) return null;
        string id = $"{companionId}_{level}";
        return CompanionLevelDataTable.TryGetValue(id, out var data) ? data : null;
    }
    public SkillData GetSkillData(string id)
    {
        if (null == SkillDataTable || string.IsNullOrEmpty(id)) return null;
        return SkillDataTable.TryGetValue(id, out var data) ? data : null;
    }
    public PlayerLevelData GetPlayerLevelData(int level)
    {
        if (null == PlayerLevelDataTable) return null;
        return PlayerLevelDataTable.TryGetValue(level.ToString(), out var data) ? data : null;
    }
    public EquipmentLevelData GetEquipmentLevelData(string id)
    {
        if (null == EquipmentLevelDataTable) return null;
        return EquipmentLevelDataTable.TryGetValue(id, out var data) ? data : null;
    }
    public EquipmentData GetEquipmentData(string id)
    {
        if (null == EquipmentDataTable || string.IsNullOrEmpty(id)) return null;
        return EquipmentDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public ConfirmData GetConfirmData(string id)
    {
        if (null == ConfirmDataTable || string.IsNullOrEmpty(id)) return null;
        return ConfirmDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public IReadOnlyList<CompanionData> GetCompanionsByGrade(int grade)
    {
        if (!_companionsByGrade.TryGetValue(grade, out List<CompanionData> companions))
        {
            Debug.LogError($"해당 등급의 동료 데이터가 없습니다. 등급 : {grade}");
            return null;
        }

        return companions;
    }

    public IReadOnlyList<EquipmentData> GetEquipmentsByGrade(int grade)
    {
        if (!_equipmentsByGrade.TryGetValue(grade, out List<EquipmentData> equipments))
        {
            Debug.LogError($"해당 등급의 장비 데이터가 없습니다 등급 : {grade}");
            return null;
        }

        return equipments;
    }

    public IReadOnlyList<GachaProbabilityData> GetGachaProbabilityData(GachaType gachaType)
    {
        if (!_probabilitiesByGachaType.TryGetValue(gachaType, out List<GachaProbabilityData> probability))
        {
            Debug.LogError($"해당 가챠 종류가 존재하지 않습니다. 종류 : {gachaType}");
            return null;
        }

        return probability;
    }

    public PerkNodeData GetPerkNodeData(string id)
    {
        if (null == PerkNodeDataTable || string.IsNullOrEmpty(id)) return null;
        return PerkNodeDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public PerkEffectData GetPerkEffectData(string id)
    {
        if (null == PerkEffectDataTable || string.IsNullOrEmpty(id)) return null;
        return PerkEffectDataTable.TryGetValue(id, out var data) ? data : null;
    }

    public WorkStatData GetWorkStatData(WorkStatType statType)
    {
        if (null == WorkStatDataTable || WorkStatType.None == statType) return null;
        return WorkStatDataTable.TryGetValue(statType.ToString(), out var data) ? data : null;
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

    private void BuildCompanionGradeIndex()
    {
        _companionsByGrade.Clear();

        foreach (CompanionData data in CompanionDataTable.Values)
        {
            if (!_companionsByGrade.TryGetValue(data.Grade, out List<CompanionData> list))
            {
                list = new List<CompanionData>();
                _companionsByGrade.Add(data.Grade, list);
            }

            list.Add(data);
        }
    }

    private void BuildGachaProbabilityIndex()
    {
        _probabilitiesByGachaType.Clear();

        foreach (GachaProbabilityData data in  GachaProbabilityDataTable.Values)
        {
            if(!_probabilitiesByGachaType.TryGetValue(data.GachaType, out List<GachaProbabilityData> list))
            {
                list = new List<GachaProbabilityData>();
                _probabilitiesByGachaType.Add(data.GachaType, list);
            }

            list.Add(data);
        }
    }

    private void BuildEquipmentGradeIndex()
    {
        _equipmentsByGrade.Clear();

        foreach (EquipmentData data in EquipmentDataTable.Values)
        {
            int grade = (int)data.EquipmentGrade;

            if (!_equipmentsByGrade.TryGetValue(grade, out List<EquipmentData> list))
            {
                list = new List<EquipmentData>();
                _equipmentsByGrade.Add(grade, list);
            }

            list.Add(data);
        }
    }
}
