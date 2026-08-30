using System;
using System.ComponentModel;

public class StageInfoViewModel
{
    private StageModel _stageModel;

    public event Action<string> OnPropertyChanged_ViewModel;

    public string StageDisplayText
    {
        get
        {
            return GetStageDisplayText();
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

    private string GetStageDisplayText()
    {
        if (_stageModel.IsBoss > 0)
        {
            return $"BOSS {_stageModel.Chapter} - {_stageModel.StageNumber}";
        }

        return $"{_stageModel.Chapter} - {_stageModel.StageNumber}";
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_stageModel.Chapter):
            case nameof(_stageModel.StageNumber):
                OnPropertyChanged_ViewModel?.Invoke(nameof(StageDisplayText));
                break;
        }
    }
}
