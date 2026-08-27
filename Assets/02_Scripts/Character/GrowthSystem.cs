using UnityEngine;

public class GrowthSystem
{
    #region 데이터 조회 (순수 계산)

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

    //public CharacterStatData GetEquipmentFinalStat(string equipmentId, int level)
    //{
    //    EquipmentLevelData levelData = GameManager.DataTable.GetEquipmentLevelData(level);
    //    EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);

    //    if (levelData == null)
    //    {
    //        Debug.LogError($"장비 레벨데이터가 없습니다. 레벨 : {level}");
    //        return null;
    //    }

    //    if (equipmentData == null)
    //    {
    //        Debug.LogError($"장비 데이터가 없습니다. 장비 : {equipmentId}");
    //        return null;
    //    }

    //    float multiplier = levelData.StatMultiplier;
    //    return new CharacterStatData(equipmentData.BaseAttack * multiplier, equipmentData.BaseDefense * multiplier, equipmentData.BaseHp * multiplier);
    //}

    #endregion

    #region 레벨업 판정

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

        return GameManager.Session.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost; 
    }

    public bool TryPayPlayerLevelUpCost(int level)
    {
        if(CheckPlayerCanLevelUp(level) == false)
        {
            return false;
        }

        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        return GameManager.Session.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
    }
    //public bool IsEquipmentMaxLevel(int level)
    //{
    //    return GameManager.DataTable.GetEquipmentLevelData(level + 1) == null;
    //}

    //public bool CheckEquipmentCanLevelUp(int level)
    //{
    //    if (IsEquipmentMaxLevel(level))
    //    {
    //        return false;
    //    }

    //    EquipmentLevelData nextLevelData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

    //    return GameManager.Session.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost;
    //}

    //public bool TryPayEquipmentLevelUpCost(int level)
    //{
    //    if(CheckEquipmentCanLevelUp(level) == false)
    //    {
    //        return false;
    //    }

    //    EquipmentLevelData nextLevelData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

    //    return GameManager.Session.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
    //}

    #endregion

    #region 전투용 조회 (현재 상태)

    //public CharacterStatData GetCharacterBattleStat(string characterId)
    //{
    //    return GetPlayerBattleStat();
    //}

    //public CharacterSkillEffectData GetCharacterBattleSkill(string characterId)
    //{
    //    if (characterId == CharacterId.PLAYER)
    //    {
    //        return GetPlayerBattleSkill();
    //    }

    //    return GetCompanionBattleSkill(characterId);
    //}

    //public CharacterStatData GetPlayerBattleStat()
    //{
    //    return GetPlayerBattleStat(GameManager.User.Player.Level);
    //}

    //public CharacterStatData GetPlayerBattleStat(int level)
    //{
    //    CharacterStatData finalStat = GetPlayerFinalStat(level);
    //    if (finalStat == null)
    //    {
    //        return null;
    //    }

    //    OwnedEquipmentData equipped = GameManager.Equipment.GetEquippedEquipment();
    //    if (equipped == null)
    //    {
    //        return finalStat;
    //    }

    //    CharacterStatData equipmentStat = GetEquipmentFinalStat(equipped.EquipmentId, equipped.Level);
    //    if(equipmentStat == null)
    //    {
    //        return finalStat;
    //    }

    //    return new CharacterStatData(finalStat.FinalAtk + equipmentStat.FinalAtk, finalStat.FinalDef + equipmentStat.FinalDef, finalStat.FinalHp + equipmentStat.FinalHp);
    //}

    //public CharacterSkillEffectData GetPlayerBattleSkill()
    //{
    //    OwnedEquipmentData equipped = GameManager.Equipment.GetEquippedEquipment();
    //    if(equipped == null)
    //    {
    //        return null;
    //    }

    //    return GetEquipmentFinalSkill(equipped.EquipmentId, equipped.Level);
    //}

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
