using UnityEngine;
using UnityEngine.UI;

public class FallingLabel : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _shapeImage;

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

    public int ShapeId { get; private set; }

    private void Reset()
    {
        _rectTransform = this.transform as RectTransform;
    }

    public void Setup(int shapeId, Sprite shapeSprite, Vector2 startPosition)
    {
        ShapeId = shapeId;

        if (null != _shapeImage && null != shapeSprite)
        {
            _shapeImage.sprite = shapeSprite;
        }

        RectTransform.anchoredPosition = startPosition;
        SetVisible(true);
    }

    public void Fall(float distance)
    {
        Vector2 position = RectTransform.anchoredPosition;
        position.y -= distance;
        RectTransform.anchoredPosition = position;
    }

    public void SnapTo(Vector2 position)
    {
        RectTransform.anchoredPosition = position;
    }

    public void SetVisible(bool isVisible)
    {
        this.gameObject.SetActive(isVisible);
    }
}
