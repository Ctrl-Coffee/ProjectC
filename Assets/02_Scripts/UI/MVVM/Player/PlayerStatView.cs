using TMPro;
using UnityEngine;

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

    [SerializeField] private UIButtonComponent _blocker;
    [SerializeField] private UIButtonComponent _btnClose;

    private PlayerStatViewModel _viewModel;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        if (_blocker != null)
        {
            _blocker.BindButtonEvent(OnClickClose);
        }

        if (_btnClose != null)
        {
            _btnClose.BindButtonEvent(OnClickClose);
        }
    }

    private void OnDisable()
    {
        UnSubscribe();
        
        if (_blocker != null)
        {
            _blocker.UnBindButtonAllEvent();
        }

        if (_btnClose != null)
        {
            _btnClose.UnBindButtonAllEvent();
        }
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

        RefreshAll();
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
        switch (propertyName)
        {
            case nameof(PlayerStatViewModel.Level):
                RefreshLevel();
                break;

            case nameof(PlayerStatViewModel.Attack):
                RefreshAttack();
                break;

            case nameof(PlayerStatViewModel.Hp):
                RefreshHp();
                break;

            case nameof(PlayerStatViewModel.Defense):
                RefreshDefense();
                break;

            case nameof(PlayerStatViewModel.CriticalRate):
                RefreshCriticalRate();
                break;

            case nameof(PlayerStatViewModel.NormalSkillHaste):
                RefreshNormalSkillHaste();
                break;

            case nameof(PlayerStatViewModel.SpecialSkillHaste):
                RefreshSpecialSkillHaste();
                break;

            case nameof(PlayerStatViewModel.CombatPower):
                RefreshCombatPower();
                break;

            default:
                RefreshAll();
                break;
        }
    }

    private void OnClickClose()
    {
        CloseUI();
    }

    private void RefreshAll()
    {
        if (_viewModel == null) return;

        _nameText.text = _viewModel.Name;

        RefreshLevel();
        RefreshCombatPower();

        RefreshAttack();
        RefreshHp();
        RefreshDefense();
        RefreshCriticalRate();
        RefreshNormalSkillHaste();
        RefreshSpecialSkillHaste();

    }

    private void RefreshLevel()
    {
        _levelText.text = $"Lv. {_viewModel.Level}";
    }

    private void RefreshCombatPower()
    {
        _combatPowerText.text = Mathf.RoundToInt(_viewModel.CombatPower).ToString("N0");
    }

    private void RefreshAttack()
    {
        _attackText.text = Mathf.RoundToInt(_viewModel.Attack).ToString();
    }

    private void RefreshHp()
    {
        _hpText.text = Mathf.RoundToInt(_viewModel.Hp).ToString();
    }

    private void RefreshDefense()
    {
        _defenseText.text = Mathf.RoundToInt(_viewModel.Defense).ToString();
    }

    private void RefreshCriticalRate()
    {
        _criticalRateText.text = $"{_viewModel.CriticalRate * Const.RATE_TO_PERCENT:0.#}%";
    }

    private void RefreshNormalSkillHaste()
    {
        _normalSkillHasteText.text = BuildHasteText(_viewModel.NormalSkillHaste, _viewModel.NormalSkillCooldownReduceRate);
    }

    private void RefreshSpecialSkillHaste()
    {
        _specialSkillHasteText.text = BuildHasteText(_viewModel.SpecialSkillHaste, _viewModel.SpecialSkillCooldownReduceRate);
    }

    private string BuildHasteText(float haste, float reduceRate)
    {
        int hasteValue = Mathf.RoundToInt(haste);
        int reducePercent = Mathf.RoundToInt(reduceRate * Const.RATE_TO_PERCENT);

        if (0 == reducePercent)
        {
            return hasteValue.ToString();
        }

        if (0 < reducePercent)
        {
            return $"{hasteValue} <color={Const.COLOR_GOOD}>(-{reducePercent}%)</color>";
        }

        return $"{hasteValue} <color={Const.COLOR_BAD}>(+{-reducePercent}%)</color>";
    }
}
