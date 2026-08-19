using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class PerkTreeZoom : MonoBehaviour, IScrollHandler
{
    [Header("참조")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content;

    [SerializeField] private Canvas _canvas;

    [Header("배율")]
    [SerializeField] private float _minScale = 0.5f;
    [SerializeField] private float _maxScale = 2f;
    [SerializeField] private float _defaultScale = 1f;

    [Header("감도")]
    [SerializeField] private float _wheelSensitivity = 0.1f;
    [SerializeField] private float _pinchSensitivity = 0.005f;

    private float _currentScale = 1f;
    private float _prevPinchDistance;
    private bool _isPinching = false;

    private bool _canScrollHorizontal;
    private bool _canScrollVertical;

#if UNITY_EDITOR
    private Vector2 _simulatedPinchOrigin;
    private bool _isSimulatingPinch = false;
#endif

    private void Awake()
    {
        if (null == _scrollRect)
        {
            _scrollRect = this.gameObject.GetComponent<ScrollRect>();
        }

        if (null == _content)
        {
            _content = _scrollRect.content;
        }

        if (null == _content)
        {
            Logger.LogError("확대/축소할 Content 가 지정되지 않았습니다.");
            enabled = false;
            return;
        }

        _canScrollHorizontal = _scrollRect.horizontal;
        _canScrollVertical = _scrollRect.vertical;

        _scrollRect.scrollSensitivity = 0f;

        SetScale(Mathf.Clamp(_defaultScale, _minScale, _maxScale));
    }

    private void OnDisable()
    {
        EndPinch();
    }

    private void Update()
    {
        UpdatePinch();
    }

    public void OnScroll(PointerEventData eventData)
    {
        float delta = eventData.scrollDelta.y;

        if (Mathf.Approximately(delta, 0f))
        {
            return;
        }

        ZoomAt(eventData.position, _currentScale * (1f + Mathf.Sign(delta) * _wheelSensitivity));
    }

    private void UpdatePinch()
    {
        if (!TryGetPinchPositions(out Vector2 first, out Vector2 second))
        {
            EndPinch();
            return;
        }

        float distance = Vector2.Distance(first, second);

        if (!_isPinching)
        {
            BeginPinch(distance);
            return;
        }

        float diff = distance - _prevPinchDistance;
        _prevPinchDistance = distance;

        if (Mathf.Approximately(diff, 0f))
        {
            return;
        }

        ZoomAt((first + second) * 0.5f, _currentScale * (1f + diff * _pinchSensitivity));
    }

    private void BeginPinch(float distance)
    {
        _isPinching = true;
        _prevPinchDistance = distance;

        _scrollRect.horizontal = false;
        _scrollRect.vertical = false;
        _scrollRect.velocity = Vector2.zero;
    }

    private void EndPinch()
    {
        if (!_isPinching)
        {
            return;
        }

        _isPinching = false;

        _scrollRect.horizontal = _canScrollHorizontal;
        _scrollRect.vertical = _canScrollVertical;
        _scrollRect.velocity = Vector2.zero;
    }

    private bool TryGetPinchPositions(out Vector2 first, out Vector2 second)
    {
        first = Vector2.zero;
        second = Vector2.zero;

#if UNITY_EDITOR
        if (TryGetSimulatedPinch(out first, out second))
        {
            return true;
        }
#endif

        Touchscreen touchscreen = Touchscreen.current;

        if (null == touchscreen)
        {
            return false;
        }

        int pressedCount = 0;

        for (int i = 0; i < touchscreen.touches.Count; i++)
        {
            TouchControl touch = touchscreen.touches[i];

            if (!touch.press.isPressed)
            {
                continue;
            }

            if (pressedCount == 0)
            {
                first = touch.position.ReadValue();
            }
            else if (pressedCount == 1)
            {
                second = touch.position.ReadValue();
            }
            else
            {
                return false;
            }

            pressedCount++;
        }

        return pressedCount == 2;
    }

#if UNITY_EDITOR
    private bool TryGetSimulatedPinch(out Vector2 first, out Vector2 second)
    {
        first = Vector2.zero;
        second = Vector2.zero;

        Keyboard keyboard = Keyboard.current;
        Mouse mouse = Mouse.current;

        if (null == keyboard || null == mouse)
        {
            return false;
        }

        if (!keyboard.altKey.isPressed || !mouse.leftButton.isPressed)
        {
            _isSimulatingPinch = false;
            return false;
        }

        Vector2 mousePosition = mouse.position.ReadValue();

        if (!_isSimulatingPinch)
        {
            _isSimulatingPinch = true;
            _simulatedPinchOrigin = mousePosition;
        }

        Vector2 offset = mousePosition - _simulatedPinchOrigin;

        first = _simulatedPinchOrigin + offset;
        second = _simulatedPinchOrigin - offset;

        return true;
    }
#endif

    private void ZoomAt(Vector2 screenPoint, float targetScale)
    {
        targetScale = Mathf.Clamp(targetScale, _minScale, _maxScale);

        if (Mathf.Approximately(targetScale, _currentScale))
        {
            return;
        }

        Camera eventCamera = GetEventCamera();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, screenPoint, eventCamera, out Vector2 beforeLocal);

        SetScale(targetScale);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_content, screenPoint, eventCamera, out Vector2 afterLocal);

        _content.anchoredPosition += (afterLocal - beforeLocal) * targetScale;
    }

    private void SetScale(float scale)
    {
        _currentScale = scale;
        _content.localScale = new Vector3(scale, scale, 1f);
    }

    private Camera GetEventCamera()
    {
        if (null == _canvas || RenderMode.ScreenSpaceOverlay == _canvas.renderMode)
        {
            return null;
        }

        return _canvas.worldCamera;
    }
}
