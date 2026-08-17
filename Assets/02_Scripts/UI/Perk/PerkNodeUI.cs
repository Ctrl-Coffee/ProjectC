using UnityEngine;
using UnityEngine.UI;

public class PerkNodeUI : MonoBehaviour
{
    [SerializeField] private string _nodeId;

    [SerializeField] private Button _button;
    [SerializeField] private Image _lockOverlay;

    [Header("상태별 가림 정도")]
    [SerializeField] private float _unlockedOverlayAlpha = 0f;
    [SerializeField] private float _unlockableOverlayAlpha = 0.35f;
    [SerializeField] private float _lockedOverlayAlpha = 0.75f;

    private PerkInfoUI _owner;
    private RectTransform _rectTransform;

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
        if (null == _owner)
        {
            return;
        }

        _owner.OnClickNode(NodeId);
    }
}
