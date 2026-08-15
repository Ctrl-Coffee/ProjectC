using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionGrowthView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _atkText;
    [SerializeField] private TextMeshProUGUI _defText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Button _levelUpButton;

    private CompanionGrowthViewModel _viewModel;

    private void OnEnable()
    {
        Subscribe();
        _levelUpButton.onClick.AddListener(OnClickLevelUp);
    }

    protected void OnDisable()
    {
        _levelUpButton.onClick.RemoveListener(OnClickLevelUp);
        UnSubscribe();
    }

    protected override void BindViewModel()
    {
        //string testID = "Companion_001"; // 테스트용 동료 ID
        //_viewModel = GameManager.ViewModel.RequestCompanionGrowthViewModel(testID);
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
    }

    protected override void UnSubscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
    }

    private void OnClickLevelUp()
    {
        if (_viewModel == null) return;

        _viewModel.LevelUp();
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        RefreshAll();
    }

    private void RefreshAll()
    {
        if (_viewModel == null) return;

        _nameText.text = _viewModel.Name;
        _levelText.text = $"Lv. {_viewModel.Level}";
        _atkText.text = Mathf.RoundToInt( _viewModel.Atk).ToString();
        _defText.text = Mathf.RoundToInt(_viewModel.Def).ToString();
        _hpText.text = Mathf.RoundToInt(_viewModel.Hp).ToString();
    }
}
