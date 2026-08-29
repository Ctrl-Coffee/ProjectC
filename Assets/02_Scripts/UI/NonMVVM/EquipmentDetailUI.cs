using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailUI : UIBase
{
    [SerializeField] private Image _iconImage;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    [SerializeField] private TextMeshProUGUI _attackText;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _defenseText;
    [SerializeField] private TextMeshProUGUI _criticalChanceText;
    [SerializeField] private TextMeshProUGUI _attackHasteText;
    [SerializeField] private TextMeshProUGUI _activeSkillHasteText;

    [SerializeField] private UIButtonComponent _levelUpButton;
    [SerializeField] private UIButtonComponent _closeButton;

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
        _nameText.text = data.Name;

        _levelUpButton.BindButtonEvent(OnClickLevelUp);

        _closeButton.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        _levelUpButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
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
        int level = GameManager.Session.HeroEquipment.GetLevel(_equipmentId);

        EquipmentLevelData levelData =
            GameManager.DataTable.GetEquipmentLevelData(Utils.GetEquipmentLevelDataId(_equipmentData.Grade, level));

        _attackText.SetText("{0}", _equipmentData.BaseAttack * levelData.StatMultiplier);
        _hpText.SetText("{0}", _equipmentData.BaseHp * levelData.StatMultiplier);
        _defenseText.SetText("{0}", _equipmentData.BaseDefense * levelData.StatMultiplier);
        _criticalChanceText.SetText("{0}", _equipmentData.BaseCriticalChance * levelData.StatMultiplier);
        _attackHasteText.SetText("{0}", _equipmentData.BasicAttackHaste * levelData.StatMultiplier);
        _activeSkillHasteText.SetText("{0}", _equipmentData.SignatureSkillHaste * levelData.StatMultiplier);

        bool isMaxLevel = levelData.UpgradeCost == 0;

        _levelUpButton.SetInteractable(!isMaxLevel);
        _levelUpButton.ChangeButtonText(isMaxLevel ? "Max Level" : "Level Up");

        if (levelData.UpgradeCost == 0)
        {
            _currencyText.SetText("Max Level");
            _levelText.SetText("Max Level");

            return;
        }

        _levelText.SetText("Lv:{0}", level);
        _currencyText.SetText("{0}/{1}", levelData.UpgradeCost, GameManager.Session.Currency.DreamFragment);
    }

}
