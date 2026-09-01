using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleVictoryPopup : UIBase
{
    [SerializeField] private TMP_Text _dreamShardRewardCount;
    [SerializeField] private TMP_Text _inspirationRewardCount;
    [SerializeField] private Button _returnPreparationButton;
    [SerializeField] private Button _nextStageButton;

    private void OnEnable()
    {
        _returnPreparationButton.onClick.AddListener(RetrunToBattlePreparation);
        _nextStageButton.onClick.AddListener(GoToNextStage);

        Refresh();
    }

    private void OnDisable()
    {
        _returnPreparationButton.onClick.RemoveListener(RetrunToBattlePreparation);
        _nextStageButton.onClick.RemoveListener(GoToNextStage);
    }

    private void Refresh()
    {
        _dreamShardRewardCount.text = $"X {GameManager.Stage.DreamShardReward}";
        _inspirationRewardCount.text = $"X {GameManager.Stage.InspirationReward}";
    }

    private void RetrunToBattlePreparation()
    {
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseBattleVictoryPopup();
    }

    private void GoToNextStage()
    {
        GameManager.Stage.SetNextStage();
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseBattleVictoryPopup();
    }
}
