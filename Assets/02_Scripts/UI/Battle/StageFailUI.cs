using UnityEngine;
using UnityEngine.UI;

public class StageFailUI : UIBase
{
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _restartStageButton;

    private void OnEnable()
    {
        _returnButton.onClick.AddListener(RetrunToBattlePreparation);
        _restartStageButton.onClick.AddListener(RestartStage);
    }

    private void OnDisable()
    {
        _returnButton.onClick.RemoveListener(RetrunToBattlePreparation);
        _restartStageButton.onClick.RemoveListener(RestartStage);
    }

    private void RetrunToBattlePreparation()
    {
        GameManager.Battle.RequestInitalizeCurrentStage();

        gameObject.SetActive(false);
    }

    private void RestartStage()
    {
        GameManager.Battle.RequestInitalizeNextStage();

        gameObject.SetActive(false);
    }
}
