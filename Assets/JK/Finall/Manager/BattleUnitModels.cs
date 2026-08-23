using System.Collections.Generic;
using UnityEngine;

public class BattleUnitModels
{
    private readonly PlayerBattleUnitModel[] _playerBattleUnitModels = new PlayerBattleUnitModel[BattleConstants.MAX_PLAYER_COUNT];
    private readonly EnemyBattleUnitModel[] _enemyBattleUnitModels = new EnemyBattleUnitModel[BattleConstants.MAX_ENEMY_COUNT];

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
            _playerBattleUnitModels[a] = new PlayerBattleUnitModel();
        }

        for (int a = 0; a < _enemyBattleUnitModels.Length; a++)
        {
            _enemyBattleUnitModels[a] = new EnemyBattleUnitModel();
        }

        BattleUnitData companionDataA = new BattleUnitData(1000f, 100f, 50f, 0.1f, 1.5f, 0.1f, 0.2f, "Skill_001", "Skill_001");
        BattleUnitData companionDataB = new BattleUnitData(800f, 120f, 40f, 0.2f, 1.8f, 0.15f, 0.1f, "Skill_001", "Skill_001");
        BattleUnitData companionDataC = new BattleUnitData(1200f, 80f, 70f, 0.05f, 1.3f, 0.05f, 0.3f, "Skill_001", "Skill_001");

        _tempCompanionModel.Add("Companion_001", companionDataA);
        _tempCompanionModel.Add("Companion_002", companionDataB);
        _tempCompanionModel.Add("Companion_003", companionDataC);
    }

    public void SetCompanion(int position, string companionId)
    {
        PlayerBattleUnitModel companionModel = _playerBattleUnitModels[position];

        if (string.IsNullOrWhiteSpace(companionId))
        {
            companionModel.Clear();
            return;
        }

        //TODO동료 모델 가져와서 만들기
        if(!_tempCompanionModel.TryGetValue(companionId, out BattleUnitData battleUnitData))
        {
            Debug.LogError("배틀 데이터 없음");
            companionModel.Clear();
            return;
        }

        companionModel.Initialize(battleUnitData);
    }

    public void RemoveCompanion(int slotIndex)
    {
        PlayerBattleUnitModel companionModel = _playerBattleUnitModels[slotIndex];

        companionModel.Clear();
    }
}




    //public void InitializeField(IReadOnlyList<string> companionIds, IReadOnlyList<string> enemyIds)
    //{
    //    InitializeHeroModel();
    //    InitializeCompanions(companionIds);
    //    InitializeEnemies(enemyIds);
    //}

    //private void InitializeHeroModel()
    //{
    //    PlayerBattleUnitModel heroBattleUnitModel = _playerBattleUnitModels[MAIN_FORMATION_INDEX];

    //    //캐릭터 모델 가져와서 만듣기
    //    BattleUnitData battleUnitData = new BattleUnitData();

    //    heroBattleUnitModel.Initialize(battleUnitData);
    //}

    //private void InitializeCompanions(IReadOnlyList<string> companionIds)
    //{
    //    if (companionIds == null)
    //    {
    //        Debug.LogError("동료 ID 목록이 null입니다.");
    //        return;
    //    }

    //    if (companionIds.Count != COMPANION_FORMATION_INDICES.Length)
    //    {
    //        Debug.LogError("동료 ID 목록 개수가 포메이션 슬롯 개수와 일치하지 않습니다.");
    //        return;
    //    }

    //    for (int index = 0; index < companionIds.Count; index++)
    //    {
    //        int formationSlotIndex = COMPANION_FORMATION_INDICES[index];

    //        PlayerBattleUnitModel companionBattleUnitModel = _playerBattleUnitModels[formationSlotIndex];

    //        string companionId = companionIds[index];

    //        if (string.IsNullOrWhiteSpace(companionId))
    //        {
    //           // companionBattleUnitModel.Clear();
    //            continue;
    //        }

    //        //동료 모델 가져와서 만듣기
    //        BattleUnitData battleUnitData = new BattleUnitData();

    //        companionBattleUnitModel.Initialize(battleUnitData);
    //    }
    //}

    //private void InitializeEnemies(IReadOnlyList<string> enemyIds)
    //{
    //    if (enemyIds == null)
    //    {
    //        Debug.LogError("적 ID 목록이 null입니다.");
    //        return;
    //    }

    //    if (enemyIds.Count != _enemyBattleUnitModels.Length)
    //    {
    //        Debug.LogError($"적 ID 목록 개수가 적 유닛 개수와 일치하지 않습니다. ");
    //        return;
    //    }

    //    for (int index = 0; index < enemyIds.Count; index++)
    //    {
    //        EnemyBattleUnitModel enemyBattleUnitModel = _enemyBattleUnitModels[index];

    //        string enemyId = enemyIds[index];

    //        if (string.IsNullOrWhiteSpace(enemyId))
    //        {
    //            continue;
    //        }


    //        //적 데이터 가져와서 만듣기
    //        BattleUnitData battleUnitData = new BattleUnitData();

    //        enemyBattleUnitModel.Initialize(battleUnitData);
    //    }
    //}

    //public BaseBattleUnitView FindEnemyTarget(int slotIndex)
    //{
    //    BaseBattleUnitView target = FindTarget(_enemyBattleUnitViews, slotIndex); 
    //    return target;
    //}

    //public BaseBattleUnitView FindPlayerTarget(int slotIndex)
    //{
    //    BaseBattleUnitView target = FindTarget(_playerBattleUnitViews, slotIndex);
    //    return target;
    //}

    //private BaseBattleUnitView FindTarget(BaseBattleUnitView[] battleUnitViews, int slotIndex)
    //{
    //    for (int index = 0; index < battleUnitViews.Length; index++)
    //    {
    //        int targetIndex = (slotIndex + index) % battleUnitViews.Length;

    //        BaseBattleUnitView battleUnitView = battleUnitViews[targetIndex];

    //        if (battleUnitView == null || battleUnitView.IsDead)
    //        {
    //            continue;
    //        }

    //        return battleUnitView;
    //    }

    //    return null;
    //}
//}
