using System.Collections.Generic;
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

    [SerializeField] private PerkBuffSlotUI[] _perkSlots;
    [SerializeField] private TextMeshProUGUI _perkEmptyText;

    [SerializeField] private Button _closeButton;

    private PlayerStatViewModel _viewModel;

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
        _viewModel.SubscribePerkChanged();

        _viewModel.InitializeModel();

        RefreshAll();
    }

    protected override void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
            _viewModel.UnSubscribePerkChanged();
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

            case nameof(PlayerStatViewModel.PerkBuffs):
                RefreshPerkBuff();
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

        RefreshPerkBuff();
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

    private void RefreshPerkBuff()
    {
        IReadOnlyList<PerkBuffInfo> perkBuffs = _viewModel.PerkBuffs;

        int buffCount = null == perkBuffs ? 0 : perkBuffs.Count;

        if (null != _perkEmptyText)
        {
            _perkEmptyText.text = Const.NO_PERK_BUFF;
            _perkEmptyText.gameObject.SetActive(0 == buffCount);
        }

        if (null == _perkSlots)
        {
            return;
        }

        for (int i = 0; i < _perkSlots.Length; i++)
        {
            PerkBuffSlotUI slot = _perkSlots[i];

            if (null == slot)
            {
                continue;
            }

            if (i < buffCount)
            {
                slot.Bind(perkBuffs[i]);
            }
            else
            {
                slot.Hide();
            }
        }

        if (_perkSlots.Length < buffCount)
        {
            Logger.LogWarning($"퍽 슬롯이 모자랍니다. 슬롯 : {_perkSlots.Length}, 퍽 : {buffCount}");
        }
    }
}
