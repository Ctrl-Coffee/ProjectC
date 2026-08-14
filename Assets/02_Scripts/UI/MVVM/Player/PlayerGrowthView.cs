using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerGrowthView : ViewBase<PlayerGrowthViewModel>
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _atkText;
    [SerializeField] private TextMeshProUGUI _defText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Button _levelUpButton;

    private void OnEnable()
    {
        _levelUpButton.onClick.AddListener(OnClickLevelUp);

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        _levelUpButton.onClick.RemoveListener(OnClickLevelUp);
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
    }
}
