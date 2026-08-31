using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : UIBase
{
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _nextStageButton;

    private void OnEnable()
    {
        _returnButton.onClick.AddListener(RetrunToBattlePreparation);
        _nextStageButton.onClick.AddListener(GoToNextStage);
    }

    private void OnDisable()
    {
        _returnButton.onClick.RemoveListener(RetrunToBattlePreparation);
        _nextStageButton.onClick.RemoveListener(GoToNextStage);
    }

    private void RetrunToBattlePreparation()
    {
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseStageClearUI();
    }

    private void GoToNextStage()
    {
        GameManager.Stage.SetNextStage();
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseStageClearUI();
    }
}
