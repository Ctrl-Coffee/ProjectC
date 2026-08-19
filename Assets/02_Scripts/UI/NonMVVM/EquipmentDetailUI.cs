using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentDetailUI : UIBase
{
    [SerializeField] private Image _iconImage;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _currencyText;

    [SerializeField] private UIButtonComponent _levelUpButton;
    [SerializeField] private UIButtonComponent _closeButton;

    private string _equipmentId;

    private System.Func<LevelUpResult> _onClickLevelUp;

    public void Init(System.Func<LevelUpResult> onClickLevelUp, EquipmentData data, string equipmentId)
    {
        _onClickLevelUp = onClickLevelUp;
        _equipmentId = equipmentId;

        Refresh();

        _iconImage.sprite = GameManager.Resource.GetLoadedAsset<Sprite>(data.IconPath);
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

        _levelText.SetText("Lv:{0}", level);

        EquipmentLevelData nextData = GameManager.DataTable.GetEquipmentLevelData(level + 1);

        _currencyText.SetText("{0}/{1}", nextData.UpgradeCost, GameManager.Session.Currency.DreamFragment);
    }

}
