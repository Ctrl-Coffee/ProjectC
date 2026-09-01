using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleHud : UIBase
{
    [Header("Timer")]
    [SerializeField] private TMP_Text _timerText;

    [Header("HpBar")]
    [SerializeField] private BattleUnitHpBarView[] _playerBattleUnitHpBarViews = new BattleUnitHpBarView[Const.MAX_PLAYER_COUNT];
    [SerializeField] private BattleUnitHpBarView[] _enemyBattleUnitHpBarViews = new BattleUnitHpBarView[Const.MAX_ENEMY_COUNT];

    [Header("BattleCharacterSlot")]
    [SerializeField] private BattleUnitStatusView[] _battleCharacterSlotViews = new BattleUnitStatusView[Const.MAX_PLAYER_COUNT];

    [Header("PauseButton")]
    [SerializeField] private UIButtonComponent _pauseButton;

    [Header("DoubleSpeedButton")]
    [SerializeField] private UIButtonComponent _doubleSpeedButton;

    private void Awake()
    {
        ValidateReferences();

        IReadOnlyList<PlayerBattleUnitModel> playerBattleUnitModels = GameManager.Battle.PlayerBattleUnitModels;
        IReadOnlyList<EnemyBattleUnitModel> enemyBattleUnitModels = GameManager.Battle.EnemyBattleUnitModels;

        SetBattleUnitModels(playerBattleUnitModels, enemyBattleUnitModels);
    }

    private void OnEnable()
    {
        BindButtonEvents();
    }

    private void LateUpdate()
    {
        UpdateTimeText();
    }

    private void OnDisable()
    {
        UnbindButtonEvents();

        ActivateBattleUnitViews();
    }

    private void ValidateReferences()
    {
        UnityUtility.ValidateReference(_timerText, nameof(_timerText));
        UnityUtility.ValidateArrayReference(_playerBattleUnitHpBarViews, nameof(_playerBattleUnitHpBarViews));
        UnityUtility.ValidateArrayReference(_enemyBattleUnitHpBarViews, nameof(_enemyBattleUnitHpBarViews));
        UnityUtility.ValidateArrayReference(_battleCharacterSlotViews, nameof(_battleCharacterSlotViews));
        UnityUtility.ValidateReference(_pauseButton, nameof(_pauseButton));
        UnityUtility.ValidateReference(_doubleSpeedButton, nameof(_doubleSpeedButton));
    }

    private void BindButtonEvents()
    {
        _pauseButton.BindButtonEvent(HandlePauseButtonButton);
        _doubleSpeedButton.BindButtonEvent(HandleDoubleSpeedButton);
    }

    private void UnbindButtonEvents()
    {
        _pauseButton.UnBindButtonAllEvent();
        _doubleSpeedButton.UnBindButtonAllEvent();
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

        for (int index = 0; index < _battleCharacterSlotViews.Length; index++)
        {
            _battleCharacterSlotViews[index].SetModel(playerBattleUnitModels[index]);
        }
    }

    private void ActivateBattleUnitViews()
    {
        for (int index = 0; index < _playerBattleUnitHpBarViews.Length; index++)
        {
            _playerBattleUnitHpBarViews[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _enemyBattleUnitHpBarViews.Length; index++)
        {
            _enemyBattleUnitHpBarViews[index].gameObject.SetActive(true);
        }

        for (int index = 0; index < _battleCharacterSlotViews.Length; index++)
        {
            _battleCharacterSlotViews[index].gameObject.SetActive(true);
        }
    }

    private void UpdateTimeText()
    {
        float battleTime = GameManager.Battle.BattleTime;
        _timerText.text = FormatTime(battleTime);
    }

    private string FormatTime(float time)
    {
        int totalSeconds = Mathf.FloorToInt(time);

        int hours = totalSeconds / 3600;
        int minutes = (totalSeconds % 3600) / 60;
        int seconds = totalSeconds % 60;

        return $"{hours:00}:{minutes:00}:{seconds:00}";
    }

    private void HandlePauseButtonButton()
    {
        GameManager.UI.OpenBattlePausePopup();
    }

    private void HandleDoubleSpeedButton()
    {
        Debug.Log("더블 눌림");
    }
}