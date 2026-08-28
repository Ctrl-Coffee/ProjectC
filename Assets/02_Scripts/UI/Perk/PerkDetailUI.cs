using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkDetailUI : UIBase
{
    private const float MIN_SLIDE_DURATION = 0.01f;

    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private TextMeshProUGUI _costText;

    [SerializeField] private UIButtonComponent _btnClose;
    [SerializeField] private UIButtonComponent _upgradeButton;
    [SerializeField] private UIButtonComponent _cancelButton;

    [Header("슬라이드 연출")]
    [SerializeField] private RectTransform _slidePanel;
    [SerializeField] private float _slideInDuration = 0.3f;
    [SerializeField] private float _slideOutDuration = 0.2f;
    [SerializeField] private float _slideDistance = 0f;

    private Button _upgradeButtonControl;

    private string _perkId;

    private Vector2 _shownPosition;
    private bool _isPositionCaptured = false;

    private RectTransform SlideTarget
    {
        get
        {
            if (null != _slidePanel)
            {
                return _slidePanel;
            }

            return _panel;
        }
    }

    private void Awake()
    {
        CapturePosition();

        if (!ValidateReferences())
        {
            return;
        }

        _btnClose.BindButtonEvent(OnClickCloseButton);
        _upgradeButton.BindButtonEvent(OnClickUpgrade);
        _cancelButton.BindButtonEvent(OnClickCancel);
    }

    private void OnDisable()
    {
        RectTransform target = SlideTarget;

        if (null != target)
        {
            target.DOKill();
        }
    }

    public override Tween PlayOpenAnimation()
    {
        if (!IsPlayAnimation)
        {
            return null;
        }

        RectTransform target = SlideTarget;

        if (null == target)
        {
            return base.PlayOpenAnimation();
        }

        target.DOKill();
        target.anchoredPosition = GetHiddenPosition();

        return target.DOAnchorPos(_shownPosition, GetDuration(_slideInDuration))
            .SetEase(Ease.OutCubic)
            .SetUpdate(true);
    }

    public override Tween PlayCloseAnimation()
    {
        RectTransform target = SlideTarget;

        if (null == target)
        {
            return base.PlayCloseAnimation();
        }

        target.DOKill();

        return target.DOAnchorPos(GetHiddenPosition(), GetDuration(_slideOutDuration))
            .SetEase(Ease.InCubic)
            .SetUpdate(true);
    }

    private void CapturePosition()
    {
        if (_isPositionCaptured)
        {
            return;
        }

        RectTransform target = SlideTarget;

        if (null == target)
        {
            Logger.LogError("슬라이드할 Panel 이 연결되지 않았습니다.");
            return;
        }

        _shownPosition = target.anchoredPosition;
        _isPositionCaptured = true;
    }

    private Vector2 GetHiddenPosition()
    {
        return new Vector2(_shownPosition.x - GetSlideDistance(), _shownPosition.y);
    }

    private float GetSlideDistance()
    {
        if (0f < _slideDistance)
        {
            return _slideDistance;
        }

        RectTransform target = SlideTarget;

        float panelWidth = target.rect.width;

        if (0f < panelWidth)
        {
            return panelWidth + Mathf.Max(0f, _shownPosition.x);
        }

        return Screen.width;
    }

    private float GetDuration(float duration)
    {
        if (duration <= 0f)
        {
            return MIN_SLIDE_DURATION;
        }

        return duration;
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

    public void SetPerk(string perkId)
    {
        if (!ValidateReferences())
        {
            return;
        }

        _perkId = perkId;

        _upgradeButton.gameObject.SetActive(true);
        _cancelButton.gameObject.SetActive(true);

        if (null == _upgradeButtonControl)
        {
            _upgradeButtonControl = _upgradeButton.gameObject.GetComponent<Button>();
        }

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

        GameManager.Sound.PlaySFX(AddressablePath.Audio.PERK_ACTIVE);

        CloseUI();
    }

    private void OnClickCancel()
    {
        if (!GameManager.Perk.TryRefund(_perkId))
        {
            Refresh();
            return;
        }

        GameManager.Sound.PlaySFX(AddressablePath.Audio.PERK_DEACTIVE);

        CloseUI();
    }
}
