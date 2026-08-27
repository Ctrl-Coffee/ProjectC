using UnityEngine;
using UnityEngine.UI;

public class StageClearUI : MonoBehaviour
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
        BattleManager.Instance.RequestInitalizeCurrentStage();

        gameObject.SetActive(false);
    }

    private void GoToNextStage()
    {
        BattleManager.Instance.RequestInitalizeNextStage();

        gameObject.SetActive(false);
    }
}
