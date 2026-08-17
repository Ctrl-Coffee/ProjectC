using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentGrowthView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _atkText;
    [SerializeField] private TextMeshProUGUI _defText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Button _levelUpButton;
    [SerializeField] private TextMeshProUGUI _skillNameText;
    [SerializeField] private TextMeshProUGUI _skillEffectText;

    private EquipmentGrowthViewModel _viewModel;

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
        //string testID = "Equipment_001";
        //_viewModel = GameManager.ViewModel.RequestEquipmentGrowthViewModel(testID);
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
        _atkText.text = Mathf.RoundToInt(_viewModel.Atk).ToString();
        _defText.text = Mathf.RoundToInt(_viewModel.Def).ToString();
        _hpText.text = Mathf.RoundToInt(_viewModel.Hp).ToString();
        _skillNameText.text = _viewModel.SkillName;
        _skillEffectText.text = $"스킬효과 : {Mathf.RoundToInt(_viewModel.SkillEffect)}";
    }
}
