using UnityEngine;

public class FallingSubtitle : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;

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

    private void Reset()
    {
        _rectTransform = this.transform as RectTransform;
    }

    public void SetPosition(Vector2 position)
    {
        RectTransform.anchoredPosition = position;
    }

    public void Fall(float distance)
    {
        Vector2 position = RectTransform.anchoredPosition;
        position.y -= distance;
        RectTransform.anchoredPosition = position;
    }
}
