using System;
using System.ComponentModel;

public class StageInfoViewModel
{
    private StageModel _stageModel;

    public event Action<string> OnPropertyChanged_ViewModel;

    public string InfoSpriteAddressableKey
    {
        get
        {
            return _stageModel.InfoSpriteAddressableKey;
        }
    }

    public string StageDisplayText
    {
        get
        {
            return GetStageDisplayText();
        }
    }

    public string RecommendedPlayerLevelText
    {
        get
        {
            return GetRecommendedPlayerLevelText();
        }
    }

    public string DreamShardRewardText
    {
        get
        {
            return GetDreamShardRewardText();
        }
    }


    public string InspirationRewardText
    {
        get
        {
            return GetInspirationRewardText();
        }
    }

    public string DpCostText
    {
        get
        {
            return GetDpCostTextText();
        }
    }

    public StageInfoViewModel() 
    {
        InitializeModel();
    }

    public void InitializeModel()
    {
        _stageModel = GameManager.Stage.StageModel;
        _stageModel.PropertyChanged += OnPropertyChanged;
    }

    public void UnBind()
    {
        _stageModel.PropertyChanged -= OnPropertyChanged;
        _stageModel = null;
    }

    public void Refresh()
    {
        _stageModel.InitializeOnce();
    }

    private string GetStageDisplayText()
    {
        if (_stageModel.IsBoss > 0)
        {
            return $"BOSS {_stageModel.Chapter}-{_stageModel.StageNumber}";
        }

        return $"{_stageModel.Chapter}-{_stageModel.StageNumber}";
    }

    private string GetRecommendedPlayerLevelText()
    {
        int recommendLevel = _stageModel.RecommendedPlayerLevel;

        int playerLevel = GameManager.Session.HeroInfo.Level;

        if (playerLevel < recommendLevel)
        {
            return $"<color={Const.STAGE_DANGER_COLOR}>LV_{recommendLevel}</color>";
        }

        return $"<color={Const.STAGE_SAFE_COLOR}>LV_{recommendLevel}</color>";
    }

    private string GetDreamShardRewardText()
    {
        return _stageModel.DreamShardReward.ToString();
    }

    private string GetInspirationRewardText()
    {
        return _stageModel.InspirationReward.ToString();
    }

    private string GetDpCostTextText()
    {
        return $"X {_stageModel.DpCost}";
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_stageModel.Chapter):
            case nameof(_stageModel.StageNumber):
                OnPropertyChanged_ViewModel?.Invoke(nameof(StageDisplayText));
                break;
            case nameof(_stageModel.RecommendedPlayerLevel):
                OnPropertyChanged_ViewModel?.Invoke(nameof(RecommendedPlayerLevelText));
                break;
            case nameof(_stageModel.DreamShardReward):
                OnPropertyChanged_ViewModel?.Invoke(nameof(DreamShardRewardText));
                break;
            case nameof(_stageModel.InspirationReward):
                OnPropertyChanged_ViewModel?.Invoke(nameof(InspirationRewardText));
                break;
            case nameof(_stageModel.DpCost):
                OnPropertyChanged_ViewModel?.Invoke(nameof(DpCostText));
                break;
            case nameof(_stageModel.InfoSpriteAddressableKey):
                OnPropertyChanged_ViewModel?.Invoke(nameof(InfoSpriteAddressableKey));
                break;
        }
    }
}
