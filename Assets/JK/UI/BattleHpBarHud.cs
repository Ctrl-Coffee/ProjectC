using System.Collections.Generic;
using UnityEngine;

public class BattleHpBarHud :UIBase
{
    [SerializeField] private BattleUnitHpBarView[] _playerBattleUnitHpBarViews = new BattleUnitHpBarView[BattleConstants.MAX_PLAYER_COUNT];
    [SerializeField] private BattleUnitHpBarView[] _enemyBattleUnitHpBarViews = new BattleUnitHpBarView[BattleConstants.MAX_ENEMY_COUNT];

    public void SetBattleUnitModels(IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels, IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels)
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