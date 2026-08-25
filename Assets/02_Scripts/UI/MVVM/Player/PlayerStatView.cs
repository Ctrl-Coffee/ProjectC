using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _combatPowerText;

    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _criticalRateText;
    [SerializeField] private TextMeshProUGUI _normalSkillHasteText;
    [SerializeField] private TextMeshProUGUI _specialSkillHasteText;

    [SerializeField] private TextMeshProUGUI _perkBuffText;

    [SerializeField] private Button _closeButton;

    private PlayerStatViewModel _viewModel;
    private StringBuilder _perkBuffBuilder = new();

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            BindViewModel();
        }

        Subscribe();
        _closeButton.onClick.AddListener(OnClickClose);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnClickClose);
        UnSubscribe();
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_viewModel != null)
        {
            _viewModel.UnBind();
            _viewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _viewModel = GameManager.ViewModel.CreatePlayerStatViewModel();
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _viewModel.InitializeModel();
    }

    protected override void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        RefreshAll();
    }

    private void OnClickClose()
    {
        CloseUI();
    }

    private void RefreshAll()
    {
        if (_viewModel == null) return;

        _nameText.text = _viewModel.Name;
        _levelText.text = $"Lv. {_viewModel.Level}";
        _combatPowerText.text = Mathf.RoundToInt(_viewModel.CombatPower).ToString("N0");

        _attackText.text = Mathf.RoundToInt(_viewModel.Attack).ToString();
        _hpText.text = Mathf.RoundToInt(_viewModel.Hp).ToString();
        _defenseText.text = Mathf.RoundToInt(_viewModel.Defense).ToString();
        _criticalRateText.text = $"{_viewModel.CriticalRate * Const.RATE_TO_PERCENT:0.#}%";
        _normalSkillHasteText.text = Mathf.RoundToInt(_viewModel.NormalSkillHaste).ToString();
        _specialSkillHasteText.text = Mathf.RoundToInt(_viewModel.SpecialSkillHaste).ToString();

        RefreshPerkBuff();
    }

    private void RefreshPerkBuff()
    {
        IReadOnlyList<PerkBuffInfo> perkBuffs = _viewModel.PerkBuffs;

        if (null == perkBuffs || 0 == perkBuffs.Count)
        {
            _perkBuffText.text = Const.NO_PERK_BUFF;
            return;
        }

        _perkBuffBuilder.Clear();

        for (int i = 0; i < perkBuffs.Count; i++)
        {
            if (0 < i)
            {
                _perkBuffBuilder.AppendLine();
            }

            _perkBuffBuilder.Append(perkBuffs[i].Name);
            _perkBuffBuilder.Append(" : ");
            _perkBuffBuilder.Append(perkBuffs[i].Value);
        }

        _perkBuffText.text = _perkBuffBuilder.ToString();
    }
}
