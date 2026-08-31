using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkBuffSlotUI : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;
    [SerializeField] private TextMeshProUGUI _valueText;
    [SerializeField] private TextMeshProUGUI _nameText;

    private string _iconKey = string.Empty;

    public void Bind(PerkBuffInfo info)
    {
        gameObject.SetActive(true);

        if (null != _nameText)
        {
            _nameText.text = info.Name;
        }

        if (null != _valueText)
        {
            _valueText.text = info.Value;
        }

        SetIcon(info.IconKey);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void SetIcon(string iconKey)
    {
        if (_iconKey == iconKey)
        {
            return;
        }

        _iconKey = iconKey;

        if (string.IsNullOrWhiteSpace(iconKey))
        {
            HideIcon();
            return;
        }

        LoadIcon(iconKey);
    }

    private void LoadIcon(string iconKey)
    {
        Sprite sprite = GameManager.Resource.GetLoadedAsset<Sprite>(iconKey);

        if (null == sprite || null == _imgIcon)
        {
            return;
        }

        _imgIcon.sprite = sprite;
        _imgIcon.enabled = true;
    }

    private void HideIcon()
    {
        if (null == _imgIcon)
        {
            return;
        }

        _imgIcon.sprite = null;
        _imgIcon.enabled = false;
    }
}
