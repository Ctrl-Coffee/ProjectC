using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitModels
{
    private readonly PlayerBattleUnitModel[] _playerBattleUnitModels = new PlayerBattleUnitModel[BattleConstants.MAX_PLAYER_COUNT];
    private readonly EnemyBattleUnitModel[] _enemyBattleUnitModels = new EnemyBattleUnitModel[BattleConstants.MAX_ENEMY_COUNT];

    private readonly IReadOnlyDictionary<string, CompanionState> _companionDictCache; 
    private readonly List<string> _saveDataPartyCompanionIds;

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

    public BattleUnitModels()
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
        //TODO 세이브 데이터에서 가져오기
        _saveDataPartyCompanionIds = new List<string> { "Companion_001", "Companion_002" };
    }

    public void InitalizeStage(string stageId)
    {
        InitializeHeroModel();
        InitializeCompanionModels();
        InitializeEnemyModels(stageId);
    }

    private void InitializeHeroModel()
    {
        PlayerBattleUnitModel heroBattleUnitModel = _playerBattleUnitModels[BattleConstants.HERO_BATTLE_POSITIONS];

        //TODO 캐릭터 모델 가져와서 만듣기
        BattleUnitData battleUnitData = BattleUtility.CreatePlayerBattleUnitData();

        heroBattleUnitModel.Initialize(battleUnitData);
    }

    private void InitializeCompanionModels()
    {
        if (_saveDataPartyCompanionIds.Count != BattleConstants.MAX_COMPANION_COUNT)
        {
            Debug.LogError("동료 ID 목록 개수가 포메이션 슬롯 개수와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _saveDataPartyCompanionIds.Count; index++)
        {
            int companionBattlePosition = BattleConstants.COMPANION_BATTLE_POSITIONS[index];

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

    private void InitializeEnemyModels(string stageId)
    {
        // TODO: stageId를 기준으로 적 ID 목록 조회
        List<string> enemyIds = new List<string>() { "enemy_ch1_001", "enemy_ch1_001", "enemy_ch1_001" };

        if (enemyIds.Count != _enemyBattleUnitModels.Length)
        {
            Debug.LogError($"스테이지 적 수({enemyIds.Count})가 시스템 최대 적 수({_enemyBattleUnitModels.Length})와 일치하지 않습니다.");
            return;
        }

        for (int index = 0; index < _enemyBattleUnitModels.Length; index++)
        {
            string enemyId = enemyIds[index];
      
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
        int index = Array.IndexOf(BattleConstants.COMPANION_BATTLE_POSITIONS, battlePosition);

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