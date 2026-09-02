using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailUI : UIBase
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Image _gradeImage;
    [SerializeField] private TextMeshProUGUI _gradeText;

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

    private EquipmentData _equipmentData;

    private string _equipmentId;

    private System.Func<LevelUpResult> _onClickLevelUp;

    public void Init(System.Func<LevelUpResult> onClickLevelUp, EquipmentData data, string equipmentId)
    {
        _equipmentData = data;

        _onClickLevelUp = onClickLevelUp;
        _equipmentId = equipmentId;

        Refresh();

        _iconImage.sprite = GameManager.Resource.GetLoadedAsset<Sprite>(data.IconSpriteAddressableKey);

        ColorUtility.TryParseHtmlString(Const.GradeColor(data.EquipmentGrade), out Color newColor);
        _gradeImage.color = newColor;

        _gradeText.text = data.Grade;
        _nameText.text = data.Name;

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
        var equipmentState = GameManager.Session.HeroEquipment.GetHeroEquipment(_equipmentId);

        EquipmentLevelData levelData =
            GameManager.DataTable.GetEquipmentLevelData(Utils.GetEquipmentLevelDataId(_equipmentData.Grade, equipmentState.Level));

        _combatPowerText.SetText("{0}", equipmentState.CombatPower);
        _attackText.SetText("{0}", _equipmentData.BaseAttack);
        _hpText.SetText("{0}", _equipmentData.BaseHp);
        _defenseText.SetText("{0}", _equipmentData.BaseDefense);
        _criticalChanceText.SetText("{0}", _equipmentData.BaseCriticalChance);
        _attackHasteText.SetText("{0}", _equipmentData.SignatureSkillHaste);
        _activeSkillHasteText.SetText("{0}", _equipmentData.SignatureSkillHaste);

        bool isMaxLevel = levelData.UpgradeCost == 0;

        _levelUpButton.SetInteractable(!isMaxLevel);
        _levelUpButton.ChangeButtonText(isMaxLevel ? "Max Level" : "Level Up");

        if (levelData.UpgradeCost == 0)
        {
            _currencyText.SetText("Max Level");
            _levelText.SetText("Max Level");

            return;
        }

        _levelText.SetText("Lv:{0}", equipmentState.Level);
        _currencyText.SetText("{0}/{1}", levelData.UpgradeCost, GameManager.Session.Currency.DreamFragment);
    }

}
