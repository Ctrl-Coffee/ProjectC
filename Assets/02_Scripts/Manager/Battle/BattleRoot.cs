using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleRoot : MonoBehaviour
{
    [Header("Background")]
    [SerializeField] private SpriteRenderer _backgroundSpriteRenderer;

    [Header("Player")]
    [SerializeField] private BattleUnitViewBase[] _playerBattleUnitViews = new BattleUnitViewBase[Const.MAX_PLAYER_COUNT];

    [Header("Enemy")]
    [SerializeField] private BattleUnitViewBase[] _enemyBattleUnitViews = new BattleUnitViewBase[Const.MAX_ENEMY_COUNT];

    private string _currentBackground;
    private bool _isBattleStarted;

    public bool IsBattleStarted
    {
        get { return _isBattleStarted; }
    }

    private void Awake()
    {
        UnityUtility.ValidateReference(_backgroundSpriteRenderer, nameof(_backgroundSpriteRenderer));
        UnityUtility.ValidateArrayReference(_playerBattleUnitViews, nameof(_playerBattleUnitViews));
        UnityUtility.ValidateArrayReference(_enemyBattleUnitViews, nameof(_enemyBattleUnitViews));
    }

    public void StartBattle()
    {
        if (_isBattleStarted)
        {
            return;
        }

        _isBattleStarted = true;

        foreach (BattleUnitViewBase playerbattleUnitView in _playerBattleUnitViews)
        {
            playerbattleUnitView.StartBattle();
        }

        foreach (BattleUnitViewBase enemybattleUnitView in _enemyBattleUnitViews)
        {
            enemybattleUnitView.StartBattle();
        }
    }

    public void EndBattle()
    {
        if (!_isBattleStarted)
        {
            return;
        }

        _isBattleStarted = false;

        foreach (BattleUnitViewBase battleUnitViewBase in _playerBattleUnitViews)
        {
            battleUnitViewBase.EndBattle();
        }

        foreach (BattleUnitViewBase battleUnitViewBase in _enemyBattleUnitViews)
        {
            battleUnitViewBase.EndBattle();
        }
    }

    public void SetBackground(string addressableKey)
    {
        if (_currentBackground == addressableKey)
        {
            return;
        }

        _currentBackground = addressableKey;

        Sprite backgroundSprite = GameManager.Resource.GetLoadedAsset<Sprite>(addressableKey);

        if (backgroundSprite == null) 
        {
            Logger.LogError($"'{addressableKey}' 로드된 배경 에셋이 없습니다.");
            return;
        }

        _backgroundSpriteRenderer.sprite = backgroundSprite;
    }

    public void InitializeBattleUnits(IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels, IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels)
    {
        InitializeBattleUnitViews(playerBattleUnitModels, _playerBattleUnitViews);
        InitializeBattleUnitViews(enemyBattleUnitModels, _enemyBattleUnitViews);
    }

    public void ResetUnitActiveState()
    {
        foreach (BattleUnitViewBase battleUnitViewBase in _playerBattleUnitViews)
        {
            battleUnitViewBase.gameObject.SetActive(true);
        }

        foreach (BattleUnitViewBase battleUnitViewBase in _enemyBattleUnitViews)
        {
            battleUnitViewBase.gameObject.SetActive(true);
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