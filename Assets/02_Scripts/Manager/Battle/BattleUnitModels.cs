using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitModels
{
    private readonly PlayerBattleUnitModel[] _playerBattleUnitModels = new PlayerBattleUnitModel[Const.MAX_PLAYER_COUNT];
    private readonly EnemyBattleUnitModel[] _enemyBattleUnitModels = new EnemyBattleUnitModel[Const.MAX_ENEMY_COUNT];

    private  IReadOnlyDictionary<string, CompanionState> _companionDictCache; 
    private  List<string> _saveDataPartyCompanionIds;

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

    public void Initialize()
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

        _companionDictCache = GameManager.Session.Companion.Companions;
        _saveDataPartyCompanionIds = new List<string> { null, null };
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

    private void InitializeHeroModel()
    {
        PlayerBattleUnitModel heroBattleUnitModel = _playerBattleUnitModels[Const.HERO_BATTLE_POSITIONS];

        HeroInfoModel heroInfo = GameManager.Session.HeroInfo;
        HeroEquipedModel heroEquiped = GameManager.Session.HeroEquiped;

        //string equipedArmorId = heroEquiped.EquipedArmorId;
        string equipedArmorId = "Equipment_Armor_001";

        EquipmentData armorData = GameManager.DataTable.GetEquipmentData(equipedArmorId);

        if (armorData == null)
        {
            Logger.LogError($"'{equipedArmorId}' 장비 데이터를 찾을 수 없습니다.");
            return;
        }

        BattleUnitData battleUnitData = BattleUtility.CreatePlayerBattleUnitData(heroInfo, armorData);

        heroBattleUnitModel.Initialize(battleUnitData);
    }

    private void InitializeCompanionModels()
    {
        if (_saveDataPartyCompanionIds.Count != Const.MAX_COMPANION_COUNT)
        {
            Debug.LogError("동료 ID 목록 개수가 포메이션 슬롯 개수와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _saveDataPartyCompanionIds.Count; index++)
        {
            int companionBattlePosition = Const.COMPANION_BATTLE_POSITIONS[index];

            string companionId = _saveDataPartyCompanionIds[index];

            if (string.IsNullOrWhiteSpace(companionId))
            {
                _playerBattleUnitModels[companionBattlePosition].Clear();
                continue;
            }

            //TODO 동료 보유 모델에서 가져오기
            //BattleUnitData battleUnitData  = BattleUtility.CreateCompanionBattleUnitData();

            //if (tempCompanionData == null)
            //{
            //    Debug.LogError($"'{companionId}' 보유 동료 모델을 찾을 수 없습니다.");

            //    _playerBattleUnitModels[companionBattlePosition].Clear();
            //    continue;
            //}

            BattleUnitData battleUnitData = BattleUtility.CreateCompanionBattleUnitData();

            _playerBattleUnitModels[companionBattlePosition].Initialize(battleUnitData);
        }
    }

    private void InitializeEnemyModels()
    {
        IReadOnlyList<string> enemyGroupIds = GameManager.Stage.EnemyGroupIds;

        if (enemyGroupIds.Count != _enemyBattleUnitModels.Length)
        {
            Debug.LogError($"스테이지 적 수({enemyGroupIds.Count})가 시스템 최대 적 수({_enemyBattleUnitModels.Length})와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _enemyBattleUnitModels.Length; index++)
        {
            string enemyId = enemyGroupIds[index];
      
            if (string.IsNullOrWhiteSpace(enemyId))
            {
                _enemyBattleUnitModels[index].Clear();
                continue;
            }

            EnemyData enemyData = GameManager.DataTable.GetEnemyData(enemyId);
           
            if (enemyData == null)
            {
                Debug.LogError($"'{enemyId}' 적 데이터를 찾을 수 없습니다.");

                _enemyBattleUnitModels[index].Clear();
                continue;
            }

            BattleUnitData battleUnitData = BattleUtility.CreateEnemyBattleUnitData(enemyData);
            _enemyBattleUnitModels[index].Initialize(battleUnitData);
        }
    }

    public void SetCompanion(int battlePosition, string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            ClearCompanion(battlePosition);
            return;
        }

        //TODO 동료 모델 가져와서 만들기
        if (!_companionDictCache.TryGetValue(companionId, out _))
        {
            Debug.LogError($"'{companionId}' 보유하지 않은 동료 아이디 입니다.");

            ClearCompanion(battlePosition);
            return;
        }

        BattleUnitData battleUnitData = BattleUtility.CreateCompanionBattleUnitData();
        _playerBattleUnitModels[battlePosition].Initialize(battleUnitData);

        UpdateSaveDataPartyCompanionId(battlePosition, companionId);
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
        UpdateSaveDataPartyCompanionId(battlePosition);
    }

    private void UpdateSaveDataPartyCompanionId(int battlePosition, string companionId = null)
    {
        int index = Array.IndexOf(Const.COMPANION_BATTLE_POSITIONS, battlePosition);

        if (index < 0)
        {
            return;
        }

        _saveDataPartyCompanionIds[index] = companionId;
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