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

        return new CharacterStatData(levelData.BaseAttack, levelData.BaseDefense, levelData.HP);
    }

    public CharacterStatData GetPlayerFinalStat(int level)
    {
        PlayerLevelData levelData = GameManager.DataTable.GetPlayerLevelData(level);

        if (levelData == null)
        {
            Debug.LogError($"플레이어 레벨 데이터를 찾을수 없습니다 레벨 : {level}");
            return null;
        }

        return new CharacterStatData(levelData.BaseAttack, levelData.BaseDefense, levelData.HP);
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
        return new CharacterStatData(equipmentData.BaseAttack * multiplier, equipmentData.BaseDefense * multiplier, equipmentData.BaseHp * multiplier);
    }

    public CharacterSkillEffectData GetEquipmentFinalSkill(string equipmentId, int level)
    {
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);
        if (equipmentData == null)
        {
            Debug.LogError($"장비 데이터가 없습니다 장비 : {equipmentId}");
            return null;
        }

        SkillData skillData = GameManager.DataTable.GetSkillData(equipmentData.SkillId);
        if (skillData == null)
        {
            Debug.LogError($"스킬 데이터가 없습니다. 스킬 : {equipmentData.SkillId}");
            return null;
        }

        EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(level);
        if (levelData == null)
        {
            Debug.LogError($"장비 레벨 데이터가 없습니다. 레벨 : {level}");
            return null;
        }

        return new CharacterSkillEffectData(skillData, skillData.BaseEffect * levelData.SkillMultiplier);
    }

    #endregion

    #region 레벨업 판정

    public bool IsCompanionMaxLevel(string companionId, int level)
    {
        return GameManager.DataTable.GetCompanionLevelData(companionId, level + 1) == null;
    }

    public bool CheckCompanionCanLevelUp(string companionId, int level)
    {
        if (IsCompanionMaxLevel(companionId, level))
        {
            return false;
        }

        CompanionLevelData nextLevelData = GameManager.DataTable.GetCompanionLevelData(companionId, level + 1);

        return GameManager.User.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost;
    }

    public bool TryPayCompanionLevelUpCost(string companionId, int level)
    {
        if (CheckCompanionCanLevelUp(companionId, level) == false)
        {
            return false;
        }

        CompanionLevelData nextLevelData = GameManager.DataTable.GetCompanionLevelData(companionId, level + 1);

        return GameManager.User.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
    }

    public bool IsPlayerMaxLevel(int level)
    {
        return GameManager.DataTable.GetPlayerLevelData(level + 1) == null;
    }

    public bool CheckPlayerCanLevelUp(int level)
    {
        if (IsPlayerMaxLevel(level))
        {
            return false;
        }

        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        return GameManager.User.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost; 
    }

    public bool TryPayPlayerLevelUpCost(int level)
    {
        if(CheckPlayerCanLevelUp(level) == false)
        {
            return false;
        }

        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        return GameManager.User.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
    }
    public bool IsEquipmentMaxLevel(int level)
    {
        return GameManager.DataTable.GetEquipmentLevelData(level + 1) == null;
    }

    public bool CheckEquipmentCanLevelUp(int level)
    {
        if (IsEquipmentMaxLevel(level))
        {
            return false;
        }

        EquipmentLevelData nextLevelData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

        return GameManager.User.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost;
    }

    public bool TryPayEquipmentLevelUpCost(int level)
    {
        if(CheckEquipmentCanLevelUp(level) == false)
        {
            return false;
        }

        EquipmentLevelData nextLevelData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

        return GameManager.User.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
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
               
        return GetEquipmentFinalSkill(equipped.EquipmentId, equipped.Level);
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
