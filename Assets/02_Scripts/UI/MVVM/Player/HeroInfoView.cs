using TMPro;
using UnityEngine;

public class HeroInfoView : ViewBase
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

    [Header("레벨업")]
    [SerializeField] private UIButtonComponent _btnLevelUp;
    [SerializeField] private TextMeshProUGUI _levelUpCostText;

    [SerializeField] private UIButtonComponent _blocker;
    [SerializeField] private UIButtonComponent _btnClose;

    private HeroInfoViewModel _viewModel;
    private CurrencyViewModel _currencyViewModel;

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

        if (_btnLevelUp != null)
        {
            _btnLevelUp.BindButtonEvent(OnClickLevelUp);
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

        if (_btnLevelUp != null)
        {
            _btnLevelUp.UnBindButtonAllEvent();
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

        if (_currencyViewModel != null)
        {
            _currencyViewModel.UnBind();
            _currencyViewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _viewModel = GameManager.ViewModel.CreateHeroInfoViewModel();
        _currencyViewModel = GameManager.ViewModel.CreateCurrencyViewModel();
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _currencyViewModel.OnPropertyChanged_ViewModel += OnCurrencyPropertyChanged;

        _viewModel.InitializeModel();

        RefreshAll();
    }

    protected override void UnSubscribe()
    {
        if (_currencyViewModel != null)
        {
            _currencyViewModel.OnPropertyChanged_ViewModel -= OnCurrencyPropertyChanged;
        }

        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(HeroInfoViewModel.Level):
                RefreshLevel();
                RefreshLevelUp();
                break;

            case nameof(HeroInfoViewModel.Attack):
                RefreshAttack();
                break;

            case nameof(HeroInfoViewModel.Hp):
                RefreshHp();
                break;

            case nameof(HeroInfoViewModel.Defense):
                RefreshDefense();
                break;

            case nameof(HeroInfoViewModel.CriticalRate):
                RefreshCriticalRate();
                break;

            case nameof(HeroInfoViewModel.NormalSkillHaste):
                RefreshNormalSkillHaste();
                break;

            case nameof(HeroInfoViewModel.SpecialSkillHaste):
                RefreshSpecialSkillHaste();
                break;

            case nameof(HeroInfoViewModel.CombatPower):
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

    private void OnCurrencyPropertyChanged(string propertyName)
    {
        if (propertyName != nameof(CurrencyViewModel.DreamFragment))
        {
            return;
        }

        if (_viewModel == null) return;

        _viewModel.RefreshLevelUpState();

        RefreshLevelUp();
    }

    private void OnClickLevelUp()
    {
        if (_viewModel == null) return;

        _viewModel.TryLevelUp();

        RefreshAll();
    }

    private void RefreshLevelUp()
    {
        if (_levelUpCostText != null)
        {
            _levelUpCostText.text = _viewModel.IsMaxLevel ? Const.MAX_LEVEL_TEXT : _viewModel.LevelUpCost.ToString("N0");
        }

        if (_btnLevelUp != null)
        {
            _btnLevelUp.SetInteractable(_viewModel.CanLevelUp);
        }
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

        RefreshLevelUp();
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
