using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoView : ViewBase
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _openRootButton;
    [SerializeField] private Button _closeButton;

    private StageInfoViewModel _stageInfoViewModel;

    private void Awake()
    {
        BindViewModel();
    }

    private void OnEnable()
    {
        Subscribe();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void OnDestroy()
    {
        UnBindViewModel();
    }

    public void SetStage(string stageId)
    {
        _stageInfoViewModel.Initialize(stageId);
    }

    protected override void BindViewModel()
    {
        _stageInfoViewModel = new StageInfoViewModel();
    }

    private void UnBindViewModel()
    {
        _stageInfoViewModel.UnBind();
        _stageInfoViewModel = null;
    }

    protected override void Subscribe()
    {
        _stageInfoViewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _openRootButton.onClick.AddListener(OpenBattleRoot);
        _closeButton.onClick.AddListener(CloseStagePopup);
    }

    protected override void UnSubscribe()
    {
        _stageInfoViewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        _openRootButton.onClick.RemoveListener(OpenBattleRoot);
        _closeButton.onClick.RemoveListener(CloseStagePopup);
    }

    private void UpdateDisplayText(string text)
    {
        _text.text = text;
    }

    private void OpenBattleRoot()
    {
        Debug.Log("루트 열기");
    }

    private void CloseStagePopup()
    {
        GameManager.UI.CloseStagePopup();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_stageInfoViewModel.StageDisplayText):
                UpdateDisplayText(_stageInfoViewModel.StageDisplayText);
                break;
        }
    }
}
