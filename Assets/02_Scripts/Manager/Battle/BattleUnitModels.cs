using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitModels
{
    private readonly PlayerBattleUnitModel[] _playerBattleUnitModels = new PlayerBattleUnitModel[Const.MAX_PLAYER_COUNT];
    private readonly EnemyBattleUnitModel[] _enemyBattleUnitModels = new EnemyBattleUnitModel[Const.MAX_ENEMY_COUNT];

    private  IReadOnlyDictionary<string, CompanionState> _companionStateDictCache; 
    private  string[] _cachedCompanionIds;

    public IReadOnlyList<PlayerBattleUnitModel> PlayerBattleUnitModels
    {
        get
        {
            return _playerBattleUnitModels;
        }
    }

    public IReadOnlyList<EnemyBattleUnitModel> EnemyBattleUnitModels
    {
        get
        {
            return _enemyBattleUnitModels;
        }
    }

    public void Initialize(CompanionPartyDto companionPartyDto)
    {
        for (int a = 0; a < _playerBattleUnitModels.Length; a++)
        {
            string unitUid = BattleUtility.CreateUniqueId();
            _playerBattleUnitModels[a] = new PlayerBattleUnitModel(a, unitUid);
        }

        for (int a = 0; a < _enemyBattleUnitModels.Length; a++)
        {
            string unitUid = BattleUtility.CreateUniqueId();
            _enemyBattleUnitModels[a] = new EnemyBattleUnitModel(a, unitUid);
        }

        _companionStateDictCache = GameManager.Session.Companion.Companions;
        _cachedCompanionIds = companionPartyDto.companionIds;
    }

    public void InitializeStage()
    {
        InitializeHeroModel();
        InitializeCompanionModels();
        InitializeEnemyModels();
    }

    public int GetAlivePlayerCount()
    {
        int count = 0;

        foreach (PlayerBattleUnitModel playerBattleUnitModel in _playerBattleUnitModels)
        {
            if (playerBattleUnitModel.IsInitialized)
            {
                count++;
            }
        }

        return count;
    }

    public int GetAliveEnemyCount()
    {
        int count = 0;

        foreach (EnemyBattleUnitModel enemyBattleUnitModel in _enemyBattleUnitModels)
        {
            if (enemyBattleUnitModel.IsInitialized)
            {
                count++;
            }
        }

        return count;
    }

    public int GetPlayerTotalCombatPower()
    {
        float totalCombatPower = 0;

        foreach (PlayerBattleUnitModel playerBattleUnitModel in _playerBattleUnitModels)
        {
            totalCombatPower += playerBattleUnitModel.CombatPower;
        }

        return (int)totalCombatPower;
    }

    public int GetEnemyTotalCombatPower()
    {
        float totalCombatPower = 0;

        foreach (EnemyBattleUnitModel enemyBattleUnitModel in _enemyBattleUnitModels)
        {
            totalCombatPower += enemyBattleUnitModel.CombatPower;
        }

        return (int)totalCombatPower;
    }

    private void InitializeHeroModel()
    {
        PlayerBattleUnitModel heroBattleUnitModel = _playerBattleUnitModels[Const.HERO_BATTLE_POSITIONS];

        HeroInfoModel heroInfo = GameManager.Session.HeroInfo;
        HeroEquipedModel heroEquiped = GameManager.Session.HeroEquiped;

       // string equipedWeaponId = heroEquiped.EquipedWeaponId;
        string equipedWeaponId = "equipment_weapon_001";

        EquipmentData armorData = GameManager.DataTable.GetEquipmentData(equipedWeaponId);

        if (armorData == null)
        {
            Logger.LogError($"'{equipedWeaponId}' 장비 데이터를 찾을 수 없습니다.");
            return;
        }

        BattleUnitData battleUnitData = BattleUtility.CreatePlayerBattleUnitData(heroInfo, armorData);

        heroBattleUnitModel.Initialize(equipedWeaponId, battleUnitData);
    }

    private void InitializeCompanionModels()
    {
        if (_cachedCompanionIds.Length != Const.MAX_COMPANION_COUNT)
        {
            Logger.LogError("동료 ID 목록 개수가 포메이션 슬롯 개수와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _cachedCompanionIds.Length; index++)
        {
            int companionBattlePosition = Const.COMPANION_BATTLE_POSITIONS[index];

            string companionId = _cachedCompanionIds[index];

            if (string.IsNullOrWhiteSpace(companionId))
            {
                _playerBattleUnitModels[companionBattlePosition].Clear();
                continue;
            }

            if (!TryCreateCompanionBattleUnitData(companionId, out BattleUnitData battleUnitData))
            {
                _playerBattleUnitModels[companionBattlePosition].Clear();
                continue;
            }

            _playerBattleUnitModels[companionBattlePosition].Initialize(companionId, battleUnitData);
        }
    }

    private void InitializeEnemyModels()
    {
        IReadOnlyList<string> enemyGroupIds = GameManager.Stage.EnemyGroupIds;

        if (enemyGroupIds.Count != _enemyBattleUnitModels.Length)
        {
            Logger.LogError($"스테이지 적 수({enemyGroupIds.Count})가 시스템 최대 적 수({_enemyBattleUnitModels.Length})와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _enemyBattleUnitModels.Length; index++)
        {
            string enemyId = enemyGroupIds[index];
      
            if (string.IsNullOrWhiteSpace(enemyId))
            {
                ClearEnemy(index);
                continue;
            }

            EnemyData enemyData = GameManager.DataTable.GetEnemyData(enemyId);
           
            if (enemyData == null)
            {
                Logger.LogError($"'{enemyId}' 적 데이터를 찾을 수 없습니다.");
                ClearEnemy(index);
                continue;
            }

            float enemyStatMultiplier = GameManager.Stage.EnemyStatMultiplier;

            BattleUnitData battleUnitData = BattleUtility.CreateEnemyBattleUnitData(enemyData, enemyStatMultiplier);
            _enemyBattleUnitModels[index].Initialize(enemyId, battleUnitData);
        }
    }

    private bool TryCreateCompanionBattleUnitData(string companionId, out BattleUnitData battleUnitData)
    {
        battleUnitData = default;

        if (!_companionStateDictCache.TryGetValue(companionId, out CompanionState companionState))
        {
            Logger.LogError($"'{companionId}' 보유하지 않은 동료 아이디 입니다.");
            return false;
        }

        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionId);

        if (companionData == null)
        {
            Logger.LogError($"'{companionId}' 동료 데이터가 없습니다.");
            return false;
        }

        battleUnitData = BattleUtility.CreateCompanionBattleUnitData(companionState, companionData);
        return true;
    }

    public void SetCompanion(int battlePosition, string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            ClearCompanion(battlePosition);
            return;
        }

        if (!_companionStateDictCache.TryGetValue(companionId, out _))
        {
            Logger.LogError($"'{companionId}' 보유하지 않은 동료 아이디 입니다.");

            ClearCompanion(battlePosition);
            return;
        }

        if (!TryCreateCompanionBattleUnitData(companionId, out BattleUnitData battleUnitData))
        {
            _playerBattleUnitModels[battlePosition].Clear();
            return;
        }

        _playerBattleUnitModels[battlePosition].Initialize(companionId, battleUnitData);

        UpdateCachedCompanionId(battlePosition, companionId);
    }

    public void RemoveCompanion(int battlePosition)
    {
        ClearCompanion(battlePosition);
    }

    public BattleUnitModelBase FindEnemyTarget(int battlePosition)
    {
        BattleUnitModelBase targetModel = FindTargetModel(_enemyBattleUnitModels, battlePosition);
        return targetModel;
    }
    
    public BattleUnitModelBase FindPlayerTarget(int battlePosition)
    {
        BattleUnitModelBase targetModel = FindTargetModel(_playerBattleUnitModels, battlePosition);
        return targetModel;
    }

    private void ClearCompanion(int battlePosition)
    {
        _playerBattleUnitModels[battlePosition].Clear();
        UpdateCachedCompanionId(battlePosition);
    }

    private void ClearEnemy(int battlePosition)
    {
        _enemyBattleUnitModels[battlePosition].Clear();
        GameManager.Battle.RequestUpdateEnemyUnitActive(battlePosition, false);
    }

    private void UpdateCachedCompanionId(int battlePosition, string companionId = null)
    {
        int index = Array.IndexOf(Const.COMPANION_BATTLE_POSITIONS, battlePosition);

        if (index < 0)
        {
            return;
        }

        _cachedCompanionIds[index] = companionId;
    }

    private BattleUnitModelBase FindTargetModel(BattleUnitModelBase[] battleUnitModels, int battlePosition)
    {
        for (int index = 0; index < battleUnitModels.Length; index++)
        {
            int targetIndex = (battlePosition + index) % battleUnitModels.Length;

            BattleUnitModelBase battleUnitModel = battleUnitModels[targetIndex];

            if (battleUnitModel.IsDead)
            {
                continue;
            }

            return battleUnitModel;
        }

        return null;
    }
}