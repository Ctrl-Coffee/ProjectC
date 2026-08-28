using System.Collections.Generic;
using UnityEngine;

public class BattleRoot : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private BattleUnitViewBase[] _playerBattleUnitViews = new BattleUnitViewBase[Const.MAX_PLAYER_COUNT];

    [Header("Enemy")]
    [SerializeField] private BattleUnitViewBase[] _enemyBattleUnitViews = new BattleUnitViewBase[Const.MAX_ENEMY_COUNT];

    private void Awake()
    {
        UnityUtility.ValidateArrayReference(_playerBattleUnitViews, nameof(_playerBattleUnitViews));
        UnityUtility.ValidateArrayReference(_enemyBattleUnitViews, nameof(_enemyBattleUnitViews));
    }

    public void StartBattle()
    {
        foreach (var a in _playerBattleUnitViews)
        {
            a.StartBattle();
        }

        foreach (var a in _enemyBattleUnitViews)
        {
            a.StartBattle();
        }
    }

    public void EndBattle()
    {
        foreach (var a in _playerBattleUnitViews)
        {
            a.EndBattle();
        }

        foreach (var a in _enemyBattleUnitViews)
        {
            a.EndBattle();
        }
    }

    public void InitializeBattleUnits(IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels, IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels)
    {
        InitializeBattleUnitViews(playerBattleUnitModels, _playerBattleUnitViews);
        InitializeBattleUnitViews(enemyBattleUnitModels, _enemyBattleUnitViews);
    }

    public void ResetUnitActiveState()
    {
        foreach (var a in _playerBattleUnitViews)
        {
            a.gameObject.SetActive(true);
        }

        foreach (var a in _enemyBattleUnitViews)
        {
            a.gameObject.SetActive(true);
        }
    }

    public void UpdateUnitActiveState(int battlePosition, bool isActive, bool isPlayer)
    {
        BattleUnitViewBase[] battleUnitViewBases = isPlayer ? _playerBattleUnitViews : _enemyBattleUnitViews;

        if (battlePosition < 0 || battlePosition >= battleUnitViewBases.Length)
        {
            Debug.LogError($"'{battlePosition}'이 뷰 배열의 유효 범위를 벗어났습니다.");
            return;
        }

        battleUnitViewBases[battlePosition].gameObject.SetActive(isActive);
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

            battleUnitViews[index].Initialize(battleUnitModels[index]);
        }
    }
}