using Unity.VisualScripting;
using UnityEngine;

public class GrowthSystem
{
    public CharacterStatData GetCompanionFinalStat(string companionId, int level)
    {
        CompanionLevelData levelData = GameManager.DataTable.GetCompanionLevelData(companionId, level);

        if (levelData == null)
        {
            Debug.LogError($"동료 레벨 데이터를 찾을 수 없습니다. Id: {companionId}, Level: {level}");
            return null;
        }

        return new CharacterStatData(levelData.ATK, levelData.DEF, levelData.HP);
    }

    public bool CheckCompanionCanLevelUp(string companionId, int level)
    {
        CompanionLevelData nextLevelData = GameManager.DataTable.GetCompanionLevelData(companionId, level + 1);

        if (nextLevelData == null)
        {
            return false;
        }

        //TODO 희준 : 꿈의 조각 확인, 차감 (nextLevelData.UpgradeCost 사용)

        return true;
    }

    public CharacterStatData GetPlayerFinalStat(int level)
    {
        PlayerLevelData levelData = GameManager.DataTable.GetPlayerLevelData(level);

        if (levelData == null)
        {
            Debug.LogError($"플레이어 레벨 데이터를 찾을수 없습니다 레벨 : {level}");
            return null;
        }

        return new CharacterStatData(levelData.ATK, levelData.DEF, levelData.HP);
    }

    public bool CheckPlayerCanLevelUp(int level)
    {
        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        if (nextLevelData == null)
        {
            return false;
        }

        //TODO 희준 : 꿈의 조각 확인, 차감 (nextLevelData.UpgradeCost 사용)

        return true;
    }

    public CharacterStatData GetEquipmentFinalStat(string equipmentId, int level)
    {
        EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(level);
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);
        
        if (levelData == null || equipmentData == null)
        {
            Debug.LogError($"장비 레벨데이터가 없습니다. 레벨 : {level}");
            return null;
        }

        if (equipmentData == null)
        {
            Debug.LogError($"장비 데이터가 없습니다. 장비 : {equipmentId}");
            return null;
        }

        float multiplier = levelData.StatMultiplier;
        return new CharacterStatData(equipmentData.BaseAtk * multiplier, equipmentData.BaseDef * multiplier, equipmentData.BaseHp * multiplier);
    }

    public bool CheckEquipmentCanLevelUp(int level)
    {
        EquipmentLevelData nextLevelData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

        if (nextLevelData == null)
        {
            return false;
        }

        //TODO 희준 : 꿈의 조각 확인, 차감 (nextLevelData.UpgradeCost 사용)
        return true;
    }
}