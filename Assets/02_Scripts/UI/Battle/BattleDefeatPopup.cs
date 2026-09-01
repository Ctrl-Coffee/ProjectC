using UnityEngine;
using UnityEngine.UI;

public class BattleDefeatPopup : UIBase
{
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartStageButton;

    private void OnEnable()
    {
        _returnButton.onClick.AddListener(RetrunToBattlePreparation);
        _restartStageButton.onClick.AddListener(RestartStage);

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

    private void UpdateRestartButton()
    {
        long dreamPoint = GameManager.Session.Currency.DreamPoint;
        long dreamPointCost = GameManager.Stage.DpCost;

        _restartStageButton.interactable = dreamPoint >= dreamPointCost;
    }

    private void RestartStage()
    {
        GameManager.Battle.RestartBattle();
        GameManager.UI.CloseBattleDefeatPopup();
    }
}
