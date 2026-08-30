using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PerkNodeUI : MonoBehaviour
{
    [SerializeField] private string _nodeId;

    [SerializeField] private Button _button;
    [SerializeField] private Image _lockOverlay;
    [SerializeField] private Image _icon;

    [Header("상태별 가림 정도")]
    [SerializeField] private float _unlockedOverlayAlpha = 0f;
    [SerializeField] private float _unlockableOverlayAlpha = 0.35f;
    [SerializeField] private float _lockedOverlayAlpha = 0.75f;

    [Header("클릭 연출")]
    [SerializeField] private float _pressScale = 0.88f;
    [SerializeField] private float _pressDuration = 0.07f;
    [SerializeField] private float _releaseDuration = 0.2f;

    private PerkInfoUI _owner;
    private RectTransform _rectTransform;
    private Sequence _pressSequence;

    public string NodeId
    {
        get
        {
            if (string.IsNullOrEmpty(_nodeId))
            {
                return this.gameObject.name;
            }

            return _nodeId;
        }
    }

    public RectTransform RectTransform
    {
        get
        {
            if (null == _rectTransform)
            {
                _rectTransform = this.transform as RectTransform;
            }

            return _rectTransform;
        }
    }

    public void Bind(PerkInfoUI owner)
    {
        _owner = owner;

        ApplyIcon();

        if (null == _button)
        {
            _button = this.gameObject.GetComponentInChildren<Button>(true);
        }

        if (null == _button)
        {
            Logger.LogWarning($"노드에 Button 이 없어 클릭을 받을 수 없습니다. id: {NodeId}");
            return;
        }

        _button.onClick.RemoveListener(OnClickNode);
        _button.onClick.AddListener(OnClickNode);
    }

    public void Refresh()
    {
        if (null == _lockOverlay)
        {
            return;
        }

        PerkNodeState state = GameManager.Perk.GetState(NodeId);
        float alpha = GetOverlayAlpha(state);

        Color color = _lockOverlay.color;
        color.a = alpha;
        _lockOverlay.color = color;
    }

    private void ApplyIcon()
    {
        if (null == _icon)
        {
            return;
        }

        if (null == _owner)
        {
            return;
        }

        PerkNodeData data = GameManager.DataTable.GetPerkNodeData(NodeId);

        if (null == data)
        {
            Logger.LogWarning($"테이블에 없는 노드라 아이콘을 지정하지 못했습니다. id: {NodeId}");
            _icon.enabled = false;
            return;
        }

        PerkNodeType nodeType = GetNodeType(data.NodeType);
        Sprite sprite = _owner.GetNodeIcon(nodeType);

        if (null == sprite)
        {
            _icon.enabled = false;
            return;
        }

        _icon.sprite = sprite;
        _icon.enabled = true;
    }

    private PerkNodeType GetNodeType(string nodeType)
    {
        if (string.IsNullOrEmpty(nodeType))
        {
            return PerkNodeType.None;
        }

        return Utils.ParseEnum<PerkNodeType>(nodeType, PerkNodeType.None);
    }

    private float GetOverlayAlpha(PerkNodeState state)
    {
        if (state == PerkNodeState.Unlocked)
        {
            return _unlockedOverlayAlpha;
        }

        if (state == PerkNodeState.Unlockable)
        {
            return _unlockableOverlayAlpha;
        }

        return _lockedOverlayAlpha;
    }

    private void OnClickNode()
    {
        PlayPressAnimation();

        GameManager.Sound.PlaySFX(AddressablePath.Audio.BUTTON_CLICK);

        if (null == _owner)
        {
            return;
        }

        _owner.OnClickNode(NodeId);
    }

    private void PlayPressAnimation()
    {
        KillPressAnimation();

        RectTransform rect = RectTransform;

        if (null == rect)
        {
            return;
        }

        rect.localScale = Vector3.one;

        _pressSequence = DOTween.Sequence();
        _pressSequence.Append(rect.DOScale(_pressScale, _pressDuration).SetEase(Ease.OutQuad));
        _pressSequence.Append(rect.DOScale(1f, _releaseDuration).SetEase(Ease.OutBack));
        _pressSequence.SetUpdate(true);
    }

    private void KillPressAnimation()
    {
        if (null == _pressSequence)
        {
            return;
        }

        if (_pressSequence.IsActive())
        {
            _pressSequence.Kill();
        }

        _pressSequence = null;
    }

    private void OnDisable()
    {
        KillPressAnimation();

        RectTransform rect = RectTransform;

        if (null != rect)
        {
            rect.localScale = Vector3.one;
        }
    }
}
