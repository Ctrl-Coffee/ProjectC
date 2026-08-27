using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleUnitModels
{
    private readonly PlayerBattleUnitModel[] _playerBattleUnitModels = new PlayerBattleUnitModel[BattleConstants.MAX_PLAYER_COUNT];
    private readonly EnemyBattleUnitModel[] _enemyBattleUnitModels = new EnemyBattleUnitModel[BattleConstants.MAX_ENEMY_COUNT];

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

    private Dictionary<string, BattleUnitData> _tempCompanionModel = new Dictionary<string, BattleUnitData>(); //Test

    public BattleUnitModels()
    {
        for (int a = 0; a < _playerBattleUnitModels.Length; a++)
        {
            _playerBattleUnitModels[a] = new PlayerBattleUnitModel(a);
        }

        for (int a = 0; a < _enemyBattleUnitModels.Length; a++)
        {
            _enemyBattleUnitModels[a] = new EnemyBattleUnitModel(a);
        }
        string unitUid1 = BattleUtility.CreateUniqueId();
        string unitUid2 = BattleUtility.CreateUniqueId();
        string unitUid3 = BattleUtility.CreateUniqueId();

        BattleUnitData companionDataA = new BattleUnitData(unitUid1, 800f, 1f, 40f, 0.2f, 1.8f, 0.15f, 0.1f, "Skill_001", "Skill_002", "AnimKey");
        BattleUnitData companionDataB = new BattleUnitData(unitUid2, 1000f, 1f, 50f, 0.1f, 1.5f, 0.1f, 0.2f, "Skill_001", "Skill_002", "AnimKey");
        BattleUnitData companionDataC = new BattleUnitData(unitUid3, 1200f, 1f, 70f, 0.05f, 1.3f, 0.05f, 0.3f, "Skill_001", "Skill_002", "AnimKey2");

        _tempCompanionModel.Add("Companion_001", companionDataA);
        _tempCompanionModel.Add("Companion_002", companionDataB);
        _tempCompanionModel.Add("Companion_003", companionDataC);

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
        BattleUnitData battleUnitData = new("Hero_001", 10000000, 10000000, 10000000, 0.05f, 1.4f, 0.05f, 0.3f, "Skill_001", "Skill_002", "AnimKey");

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
            BattleUnitData tempCompanionData = _tempCompanionModel[companionId];

            //if (tempCompanionData == null)
            //{
            //    Debug.LogError($"'{companionId}' 보유 동료 모델을 찾을 수 없습니다.");

            //    _playerBattleUnitModels[companionBattlePosition].Clear();
            //    continue;
            //}

            //BattleUnitData battleUnitData = tempCompanionData;

            _playerBattleUnitModels[companionBattlePosition].Initialize(tempCompanionData);
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

        //TODO 데이터 다듬기
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

            string unitUid = BattleUtility.CreateUniqueId();

            BattleUnitData battleUnitData = new(unitUid, enemyData.BaseHP, enemyData.BaseATK, enemyData.BaseDEF, 0.05f, enemyData.AttackInterval, 0.05f, 0.3f, "Skill_001", "Skill_002", "AnimKey");
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
        if (!_tempCompanionModel.TryGetValue(companionId, out BattleUnitData battleUnitData))
        {
            Debug.LogError("배틀 데이터 없음");

            ClearCompanion(battlePosition);
            return;
        }

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