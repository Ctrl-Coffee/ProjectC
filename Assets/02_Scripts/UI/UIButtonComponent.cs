using UnityEngine;
using UnityEngine.UI;

public class UIButtonComponent : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Text _text;

    private void Awake()
    {
        InitUIButton();
    }

    private void InitUIButton()
    {
        if (_button != null)
        {
            return;
        }

        var button = this.gameObject.GetComponentInChildren<Button>();
        if (button != null)
        {
            this._button = button;
        }
    }

    public void BindButtonEvent(System.Action onClickCallback)
    {
        if (_button == null) return;

        _button.onClick.AddListener(onClickCallback.Invoke);
    }

    public void UnBindButtonAllEvent()
    {
        if (_button == null) return;

        _button.onClick.RemoveAllListeners();
    }

    public void ChangeButtonText(string buttonStr)
    {
        if (_text == null) return;

        _text.text = buttonStr;
    }
}
