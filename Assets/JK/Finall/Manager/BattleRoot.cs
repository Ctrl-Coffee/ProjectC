using System.Collections.Generic;
using UnityEngine;

public class BattleRoot : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private BattleUnitViewBase[] _playerBattleUnitViews = new BattleUnitViewBase[BattleConstants.MAX_PLAYER_COUNT];

    [Header("Enemy")]
    [SerializeField] private BattleUnitViewBase[] _enemyBattleUnitViews = new BattleUnitViewBase[BattleConstants.MAX_ENEMY_COUNT];

    private void Awake()
    {
        UnityUtility.ValidateArrayReference(_playerBattleUnitViews, nameof(_playerBattleUnitViews));
        UnityUtility.ValidateArrayReference(_enemyBattleUnitViews, nameof(_enemyBattleUnitViews));
    }

    public void InitializeBattleUnits(IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels, IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels)
    {
        InitializeBattleUnitViews(playerBattleUnitModels, _playerBattleUnitViews);
        InitializeBattleUnitViews(enemyBattleUnitModels, _enemyBattleUnitViews);
    }

    private void InitializeBattleUnitViews(IReadOnlyList<BattleUnitModelBase> battleUnitModels, IReadOnlyList<BattleUnitViewBase> battleUnitViews)
    {
        if (battleUnitModels == null || battleUnitViews == null)
        {
            Debug.LogError("배틀 유닛 Model 또는 View가 null입니다.");
            return;
        }

        if (battleUnitModels.Count != battleUnitViews.Count)
        {
            Debug.LogError($"배틀 유닛 뷰와 모델 개수가 일치하지 않습니다. Model: {battleUnitModels.Count}개, View: {battleUnitViews.Count}개");
            return;
        }

        for (int index = 0; index < battleUnitViews.Count; index++)
        {
            if (battleUnitViews[index] == null)
            {
                Debug.LogError($"'[{index}]' 배틀 유닛 View가 null입니다.");
                continue;
            }

            if (battleUnitModels[index] == null)
            {
                Debug.LogError($"'[{index}]' 배틀 유닛 Model이 null입니다.");
                continue;
            }

            battleUnitViews[index].Initialize(index, battleUnitModels[index]);
        }
    }

    //public void StartBattle()
    //{
    //    foreach (var a in _playerBattleUnitViews)
    //    {
    //        a.StartBattle();
    //    }

    //    foreach (var a in _enemyBattleUnitViews)
    //    {
    //        a.StartBattle();
    //    }
    //}

    //public void InitializeField(string mainId, IReadOnlyList<string> companionIds, IReadOnlyList<string> enemyIds)
    //{
    //    InitializeMain(mainId);
    //    InitializeCompanions(companionIds);
    //    InitializeEnemies(enemyIds);
    //}



    //private void InitializeMain(string mainId)
    //{
    //    BaseBattleUnitView mainBattleUnitView = _playerBattleUnitViews[MAIN_FORMATION_INDEX];

    //    mainBattleUnitView.Initialize(mainId, MAIN_FORMATION_INDEX);
    //    mainBattleUnitView.gameObject.SetActive(true);
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

    //        BaseBattleUnitView companionBattleUnitView = _playerBattleUnitViews[formationSlotIndex];

    //        string companionId = companionIds[index];

    //        if (string.IsNullOrWhiteSpace(companionId))
    //        {
    //            companionBattleUnitView.gameObject.SetActive(false);
    //            continue;
    //        }

    //        companionBattleUnitView.Initialize(companionId, formationSlotIndex);
    //        companionBattleUnitView.gameObject.SetActive(true);
    //    }
    //}

    //private void InitializeEnemies(IReadOnlyList<string> enemyIds)
    //{
    //    if (enemyIds == null)
    //    {
    //        Debug.LogError("적 ID 목록이 null입니다.");
    //        return;
    //    }

    //    if (enemyIds.Count != _enemyBattleUnitViews.Length)
    //    {
    //        Debug.LogError($"적 ID 목록 개수가 적 유닛 개수와 일치하지 않습니다. ");
    //        return;
    //    }

    //    for (int index = 0; index < enemyIds.Count; index++)
    //    {
    //        BaseBattleUnitView enemyBattleUnitView = _enemyBattleUnitViews[index];

    //        string enemyId = enemyIds[index];

    //        if (string.IsNullOrWhiteSpace(enemyId))
    //        {
    //            enemyBattleUnitView.gameObject.SetActive(false);
    //            continue;
    //        }

    //        enemyBattleUnitView.Initialize(enemyId, index);
    //        enemyBattleUnitView.gameObject.SetActive(true);
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
}