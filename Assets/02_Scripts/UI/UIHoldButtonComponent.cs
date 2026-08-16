using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoldButtonComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool _isPressed = false;

    public bool IsPressed
    {
        get
        {
            return _isPressed;
        }
    }

    private void OnDisable()
    {
        _isPressed = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isPressed = false;
    }

    public void ResetButtonPress()
    {
        _isPressed = false;
    }
}
