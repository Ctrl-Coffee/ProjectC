using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDefeatPopup : UIBase
{
    [Header("BackgroundSprites")]
    [SerializeField] private List<Sprite> _backgroundSprites = new List<Sprite>();

    [Header("Components")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartStageButton;

    private void OnEnable()
    {
        _returnButton.onClick.AddListener(RetrunToBattlePreparation);
        _restartStageButton.onClick.AddListener(RestartStage);

        UpdateSprite();
        UpdateRestartButton();
    }

    private void OnDisable()
    {
        _returnButton.onClick.RemoveListener(RetrunToBattlePreparation);
        _restartStageButton.onClick.RemoveListener(RestartStage);
    }

    private void RetrunToBattlePreparation()
    {
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseBattleDefeatPopup();
    }

    private void UpdateSprite()
    {
        int chapter = GameManager.Stage.Chapter;
        int spriteIndex = chapter - 1;

        _backgroundImage.sprite = _backgroundSprites[spriteIndex];
    }

    private void UpdateRestartButton()
    {
        long dreamPoint = GameManager.Session.Currency.DreamPoint;
        long dreamPointCost = GameManager.Stage.DpCost;

        _restartStageButton.interactable = dreamPoint >= dreamPointCost;
    }

    private void RestartStage()
    {
        GameManager.Battle.RestartDefeatBattle();
        GameManager.UI.CloseBattleDefeatPopup();
    }
}
