using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageInfoView : ViewBase
{
    [Header("UI Reference")]
    [SerializeField] private Image _stageImage;
    [SerializeField] private TMP_Text _stageGradeText;
    [SerializeField] private TMP_Text _recommendLevelText;
    [SerializeField] private TMP_Text _rewardDreamFragmentText;
    [SerializeField] private TMP_Text _rewardInspirationText;
    [SerializeField] private TMP_Text _dpCostText;

    [Header("Button")]
    [SerializeField] private Button _openRootButton;
    [SerializeField] private Button _closeButton;

    private StageInfoViewModel _stageInfoViewModel;

    private void Awake()
    {
        ValidateReferences();
        BindViewModel();
    }

    private void OnEnable()
    {
        Subscribe();

        Refresh();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void OnDestroy()
    {
        UnBindViewModel();
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

    private void ValidateReferences()
    {
        UnityUtility.ValidateReference(_stageImage, nameof(_stageImage));
        UnityUtility.ValidateReference(_stageGradeText, nameof(_stageGradeText));
        UnityUtility.ValidateReference(_recommendLevelText, nameof(_recommendLevelText));
        UnityUtility.ValidateReference(_rewardDreamFragmentText, nameof(_rewardDreamFragmentText));
        UnityUtility.ValidateReference(_rewardInspirationText, nameof(_rewardInspirationText));
        UnityUtility.ValidateReference(_dpCostText, nameof(_dpCostText));
    }

    private void Refresh()
    {
        _stageInfoViewModel.Refresh();
    }

    private void UpdateStageImage(string addressKey)
    {
        Sprite sprite = GameManager.Resource.GetLoadedAsset<Sprite>(addressKey);
        _stageImage.sprite = sprite;
    }

    private void UpdateStageGradeText(string text)
    {
        _stageGradeText.text = text;
    }

    private void UpdateRecommendLevelText(string text)
    {
        _recommendLevelText.text = text;
    }

    private void UpdateRewardDreamFragmentText(string text)
    {
        _rewardDreamFragmentText.text = text;
    }

    private void UpdateRewardInspiration(string text)
    {
        _rewardInspirationText.text = text;
    }

    private void UpdateDpCostText(string text)
    {
        _dpCostText.text = text;
    }

    private void UpdateOpenRootButtonState()
    {
        long dreamPoint = GameManager.Session.Currency.DreamPoint;
        long dreamPointCost = GameManager.Stage.DpCost;

        _openRootButton.interactable = dreamPoint >= dreamPointCost;
    }

    private void OpenBattleRoot()
    {
        GameManager.Battle.EnterBattle();
        GameManager.Instance.ExitDream();
        GameManager.UI.CloseStagePopup();
    }

    private void CloseStagePopup()
    {
        GameManager.UI.CloseStagePopup();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_stageInfoViewModel.InfoSpriteAddressableKey):
                UpdateStageImage(_stageInfoViewModel.InfoSpriteAddressableKey);
                break;
            case nameof(_stageInfoViewModel.StageDisplayText):
                UpdateStageGradeText(_stageInfoViewModel.StageDisplayText);
                break;
            case nameof(_stageInfoViewModel.RecommendedPlayerLevelText):
                UpdateRecommendLevelText(_stageInfoViewModel.RecommendedPlayerLevelText);
                break;
            case nameof(_stageInfoViewModel.DreamShardRewardText):
                UpdateRewardDreamFragmentText(_stageInfoViewModel.DreamShardRewardText);
                break;
            case nameof(_stageInfoViewModel.InspirationRewardText):
                UpdateRewardInspiration(_stageInfoViewModel.InspirationRewardText);
                break;
            case nameof(_stageInfoViewModel.DpCostText):
                UpdateDpCostText(_stageInfoViewModel.DpCostText);
                UpdateOpenRootButtonState();
                break;
        }
    }
}
