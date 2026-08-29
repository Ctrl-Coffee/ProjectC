using TMPro;
using UnityEngine;

public class HeroInfoView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _combatPowerText;

    [Header("스텟")]
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _criticalChanceText;
    [SerializeField] private TextMeshProUGUI _basicAttackHasteText;
    [SerializeField] private TextMeshProUGUI _basicActiveSkillHasteText;

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

            case nameof(HeroInfoViewModel.CriticalChance):
                RefreshCriticalChance();
                break;

            case nameof(HeroInfoViewModel.BasicAttackHaste):
                RefreshBasicAttackHaste();
                break;

            case nameof(HeroInfoViewModel.BasicActiveSkillHaste):
                RefreshBasicActiveSkillHaste();
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
        RefreshCriticalChance();
        RefreshBasicAttackHaste();
        RefreshBasicActiveSkillHaste();

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
        _attackText.text = BuildStatText(_viewModel.Attack, _viewModel.EquipmentStat.Attack);
    }

    private void RefreshHp()
    {
        _hpText.text = BuildStatText(_viewModel.Hp, _viewModel.EquipmentStat.Hp);
    }

    private void RefreshDefense()
    {
        _defenseText.text = BuildStatText(_viewModel.Defense, _viewModel.EquipmentStat.Defense);
    }

    private void RefreshCriticalChance()
    {
        _criticalChanceText.text = BuildPercentStatText(_viewModel.CriticalChance, _viewModel.EquipmentStat.CriticalChance);
    }

    private void RefreshBasicAttackHaste()
    {
        _basicAttackHasteText.text = BuildHasteText(
            _viewModel.BasicAttackHaste, _viewModel.EquipmentStat.BasicAttackHaste, _viewModel.BasicAttackCooldownReduceRate);
    }

    private void RefreshBasicActiveSkillHaste()
    {
        _basicActiveSkillHasteText.text = BuildHasteText(
            _viewModel.BasicActiveSkillHaste, _viewModel.EquipmentStat.BasicActiveSkillHaste, _viewModel.BasicActiveSkillCooldownReduceRate);
    }

    private string BuildStatText(float total, float equipment)
    {
        int totalValue = Mathf.RoundToInt(total);
        int equipmentValue = Mathf.RoundToInt(equipment);

        if (0 == equipmentValue)
        {
            return totalValue.ToString();
        }

        int baseValue = totalValue - equipmentValue;

        return $"{totalValue} ({baseValue} + <color={Const.COLOR_GOOD}>{equipmentValue}</color>)";
    }

    private string BuildPercentStatText(float totalRate, float equipmentRate)
    {
        float totalPercent = totalRate * Const.RATE_TO_PERCENT;
        float equipmentPercent = equipmentRate * Const.RATE_TO_PERCENT;

        if (Mathf.Approximately(0f, equipmentPercent))
        {
            return $"{totalPercent:0.#}%";
        }

        float basePercent = totalPercent - equipmentPercent;

        return $"{totalPercent:0.#}% ({basePercent:0.#}% + <color={Const.COLOR_GOOD}>{equipmentPercent:0.#}%</color>)";
    }

    private string BuildHasteText(float haste, float equipmentHaste, float reduceRate)
    {
        string valueText = BuildStatText(haste, equipmentHaste);

        float reducePercent = reduceRate * Const.RATE_TO_PERCENT;

        if (0f < reducePercent)
        {
            return $"{valueText} <color={Const.COLOR_GOOD}>(-{reducePercent:0.#}%)</color>";
        }

        if (0f > reducePercent)
        {
            return $"{valueText} <color={Const.COLOR_BAD}>(+{-reducePercent:0.#}%)</color>";
        }

        return $"{valueText} (0%)";
    }
}
