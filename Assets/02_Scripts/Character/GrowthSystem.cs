using UnityEngine;

public class GrowthSystem
{
    #region 데이터 조회 (순수 계산)

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

    public CharacterStatData GetEquipmentFinalStat(string equipmentId, int level)
    {
        EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(level);
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);

        if (levelData == null)
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

    #endregion

    #region 레벨업 판정

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

    #endregion

    #region 전투용 조회 (현재 상태)

    public CharacterStatData GetCharacterBattleStat(string characterId)
    {
        if (characterId == CharacterId.PLAYER)
        {
            return GetPlayerBattleStat();
        }

        return GetCompanionBattleStat(characterId);
    }

    public CharacterSkillEffectData GetCharacterBattleSkill(string characterId)
    {
        if (characterId == CharacterId.PLAYER)
        {
            return GetPlayerBattleSkill();
        }

        return GetCompanionBattleSkill(characterId);
    }

    public CharacterStatData GetPlayerBattleStat()
    {
        return GetPlayerBattleStat(GameManager.User.Player.Level);
    }

    public CharacterStatData GetPlayerBattleStat(int level)
    {
        CharacterStatData finalStat = GetPlayerFinalStat(level);
        if (finalStat == null)
        {
            return null;
        }

        OwnedEquipmentData equipped = GameManager.Equipment.GetEquippedEquipment();
        if (equipped == null)
        {
            return finalStat;
        }

        CharacterStatData equipmentStat = GetEquipmentFinalStat(equipped.EquipmentId, equipped.Level);
        if(equipmentStat == null)
        {
            return finalStat;
        }

        return new CharacterStatData(finalStat.FinalAtk + equipmentStat.FinalAtk, finalStat.FinalDef + equipmentStat.FinalDef, finalStat.FinalHp + equipmentStat.FinalHp);
    }

    public CharacterStatData GetCompanionBattleStat(string companionId)
    {
        OwnedCompanionData owned = GameManager.Companion.GetOwnedCompanion(companionId);
        if (owned == null)
        {
            Debug.LogError($"보유하지 않은 동료. id : {companionId}");
            return null;
        }

        return GetCompanionFinalStat(companionId, owned.Level);
    }

    public CharacterSkillEffectData GetPlayerBattleSkill()
    {
        OwnedEquipmentData equipped = GameManager.Equipment.GetEquippedEquipment();
        if(equipped == null)
        {
            return null;
        }

        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipped.EquipmentId);
        if (equipmentData == null)
        {
            Debug.LogError($"장비 데이터가 없습니다 장비 : {equipped.EquipmentId}");
            return null;
        }

        SkillData skillData = GameManager.DataTable.GetSkillData(equipmentData.SkillId);
        if (skillData == null)
        {
            Debug.LogError($"스킬 데이터가 없습니다. 스킬 : {skillData.Id}");
            return null;
        }

        EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(equipped.Level);
        if (levelData == null)
        {
            Debug.LogError($"장비 레벨 데이터가 없습니다. 레벨 : {equipped.Level}");
            return null;
        }

        return new CharacterSkillEffectData(skillData, skillData.BaseEffect * levelData.SkillMultiplier);
    }

    public CharacterSkillEffectData GetCompanionBattleSkill(string companionId)
    {
        CompanionData companion = GameManager.DataTable.GetCompanionData(companionId);
        if (companion == null)
        {
            Debug.LogError($"동료 데이터가 없습니다. 동료 : {companionId}");
            return null;
        }

        SkillData skillData = GameManager.DataTable.GetSkillData(companion.SkillId);
        if (skillData == null)
        {
            Debug.LogError($"스킬 데이터가 없습니다. 스킬 : {companion.SkillId}");
            return null;
        }

        return new CharacterSkillEffectData(skillData, skillData.BaseEffect);
    }

    #endregion
}
