using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkDetailUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;

    [SerializeField] private UIButtonComponent _btnClose;
    [SerializeField] private UIButtonComponent _upgradeButton;
    [SerializeField] private UIButtonComponent _cancelButton;

    private Button _upgradeButtonControl;

    private string _perkId;
    private PerkInfoUI _owner;

    private void OnEnable()
    {
        if (!ValidateReferences())
        {
            return;
        }

        _btnClose.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        if (null == _btnClose)
        {
            return;
        }

        _btnClose.UnBindButtonAllEvent();
    }

    private bool ValidateReferences()
    {
        if (!IsAssigned(_nameText, nameof(_nameText)))
        {
            return false;
        }

        if (!IsAssigned(_descriptionText, nameof(_descriptionText)))
        {
            return false;
        }

        if (!IsAssigned(_costText, nameof(_costText)))
        {
            return false;
        }

        if (!IsAssigned(_btnClose, nameof(_btnClose)))
        {
            return false;
        }

        if (!IsAssigned(_upgradeButton, nameof(_upgradeButton)))
        {
            return false;
        }

        if (!IsAssigned(_cancelButton, nameof(_cancelButton)))
        {
            return false;
        }

        return true;
    }

    private bool IsAssigned(UnityEngine.Object reference, string fieldName)
    {
        if (null != reference)
        {
            return true;
        }

        Logger.LogError($"{fieldName} 이 연결되지 않았습니다.");
        return false;
    }

    public void SetPerk(string perkId, PerkInfoUI owner)
    {
        if (!ValidateReferences())
        {
            return;
        }

        _perkId = perkId;
        _owner = owner;

        _upgradeButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(true);

        if (null == _upgradeButtonControl)
        {
            _upgradeButtonControl = _upgradeButton.gameObject.GetComponent<Button>();
        }

        _upgradeButton.UnBindButtonAllEvent();
        _cancelButton.UnBindButtonAllEvent();

        _upgradeButton.BindButtonEvent(OnClickUpgrade);
        _cancelButton.BindButtonEvent(OnClickCancel);

        Refresh();
    }

    private void Refresh()
    {
        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(_perkId);

        if (null == data)
        {
            Logger.LogError($"테이블에 없는 퍽입니다. id: {_perkId}");
            CloseUI();
            return;
        }

        _nameText.text = data.Name;
        _descriptionText.text = data.Description;
        _costText.text = data.InspirationCost.ToString();

        bool isUnlocked = GameManager.Perk.IsUnlocked(_perkId);
        bool canUnlock = GameManager.Perk.CanUnlock(_perkId, out string _);
        bool canRefund = GameManager.Perk.CanRefund(_perkId, out string _);

        _upgradeButton.gameObject.SetActive(!isUnlocked);
        _cancelButton.gameObject.SetActive(isUnlocked && canRefund);

        if (null != _upgradeButtonControl)
        {
            _upgradeButtonControl.interactable = canUnlock;
        }
    }

    private void OnClickUpgrade()
    {
        if (!GameManager.Perk.TryUnlock(_perkId))
        {
            Refresh();
            return;
        }

        NotifyOwner();
        CloseUI();
    }

    private void OnClickCancel()
    {
        if (!GameManager.Perk.TryRefund(_perkId))
        {
            Refresh();
            return;
        }

        NotifyOwner();
        CloseUI();
    }

    private void NotifyOwner()
    {
        if (null == _owner)
        {
            return;
        }

        _owner.RefreshAll();
    }
}
