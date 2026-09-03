using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CompanionDetailUI : UIBase
{
    [SerializeField] private Image _iconImage;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    [SerializeField] private TextMeshProUGUI _combatPowerText;
    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _criticalChanceText;
    [SerializeField] private TextMeshProUGUI _attackHasteText;
    [SerializeField] private TextMeshProUGUI _activeSkillHasteText;

    [SerializeField] private UIButtonComponent _levelUpButton;
    [SerializeField] private UIButtonComponent _closeButton;
    [SerializeField] private UIButtonComponent _darkBGButton;

    private System.Func<LevelUpResult> _onClickLevelUp;

    private CompanionState _companionState;

    public void Init(CompanionState companionState, System.Func<LevelUpResult> onClickLevelUp)
    {
        _companionState = companionState;

        Refresh();

        _onClickLevelUp = onClickLevelUp;

        var companionData = GameManager.DataTable.GetCompanionData(companionState.CompanionId);

        _iconImage.sprite = GameManager.Resource.GetLoadedAsset<SpriteAtlas>(AddressablePath.Atlas.CompanionFullBody)
            .GetSprite(companionState.CompanionId);
        _nameText.text = companionData.Name;

        _levelUpButton.BindButtonEvent(OnClickLevelUp);
        _closeButton.BindButtonEvent(OnClickCloseButton);
        _darkBGButton.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        _levelUpButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
        _darkBGButton.UnBindButtonAllEvent();
    }

    private void OnClickLevelUp()
    {
        var result = _onClickLevelUp?.Invoke();

        if (result == LevelUpResult.Success)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        var levelData = GameManager.DataTable.GetCompanionLevelData(_companionState.Level);

        _combatPowerText.SetText("{0}", _companionState.CombatPower);
        _attackText.SetText("{0}", _companionState.Attack);
        _hpText.SetText("{0}", _companionState.Hp);
        _defenseText.SetText("{0}", _companionState.Defense);
        _criticalChanceText.SetText("{0}", _companionState.CriticalChance);
        _attackHasteText.SetText("{0}", _companionState.BasicAttackHaste);
        _activeSkillHasteText.SetText("{0}", _companionState.SignatureSkillHaste);

        bool isMaxLevel = levelData.UpgradeCost == 0;

        _levelUpButton.SetInteractable(!isMaxLevel);
        _levelUpButton.ChangeButtonText(isMaxLevel ? "Max Level" : "Level Up");

        if (levelData.UpgradeCost == 0)
        {
            _currencyText.SetText("Max Level");
            _levelText.SetText("Max Level");

            return;
        }

        _levelText.SetText("Lv:{0}", _companionState.Level);
        _currencyText.SetText("{0}/{1}", GameManager.Session.Currency.DreamFragment, levelData.UpgradeCost);
    }
}
