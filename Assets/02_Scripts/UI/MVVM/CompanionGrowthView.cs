using TMPro;
using UnityEngine;

public class CompanionGrowthView : ViewBase<CompanionGrowthViewModel>
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _atkText;
    [SerializeField] private TextMeshProUGUI _defText;
    [SerializeField] private TextMeshProUGUI _hpText;

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
