using UnityEngine;

public class PerkNodeUI : MonoBehaviour
{
    [SerializeField] private string _nodeId;

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
}
