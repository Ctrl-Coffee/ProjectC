using System.Collections.Generic;
using UnityEngine;

public class BattleHpBarHud : UIBase
{
    [SerializeField] private BattleUnitHpBarView[] _playerBattleUnitHpBarViews = new BattleUnitHpBarView[Const.MAX_PLAYER_COUNT];
    [SerializeField] private BattleUnitHpBarView[] _enemyBattleUnitHpBarViews = new BattleUnitHpBarView[Const.MAX_ENEMY_COUNT];

    private void Start()
    {
        IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels = GameManager.Battle.PlayerBattleUnitModels;
        IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels = GameManager.Battle.EnemyBattleUnitModels;

        SetBattleUnitModels(playerBattleUnitModels, enemyBattleUnitModels);
    }

    private void SetBattleUnitModels(IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels, IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels)
    {
        for (int index = 0; index < _playerBattleUnitHpBarViews.Length; index++)
        {
            _playerBattleUnitHpBarViews[index].SetModel(playerBattleUnitModels[index]);
        }

        for (int index = 0; index < _enemyBattleUnitHpBarViews.Length; index++)
        {
            _enemyBattleUnitHpBarViews[index].SetModel(enemyBattleUnitModels[index]);
        }
    }
}