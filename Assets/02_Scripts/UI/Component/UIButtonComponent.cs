using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonComponent : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake()
    {
        InitUIButton();
    }

    private void InitUIButton()
    {
        if (_button == null)
        {
            _button = this.gameObject.GetComponent<Button>();
        }

        if(_text == null)
        {
            _text = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
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

    private void Reset()
    {
        InitUIButton();
    }
}
