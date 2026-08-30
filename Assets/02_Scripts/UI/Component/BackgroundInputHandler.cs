using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BackgroundInputHandler : MonoBehaviour
{
    [Header("Tap Settings")]
    [SerializeField] private float _tapDistanceRate = 0.02f;

    private Vector2 _startPointerPosition;

    private bool _isPointerPressed;
    private bool _isDragging;
    private bool _isTouchInput;
    private bool _isInteractionBlocked;

    private Camera _camera;

    public event Action<Vector2> DragStarted;
    public event Action<Vector2> Dragged;
    public event Action<Vector2> DragEnded;
    public event Action<Vector2> Tapped;
    public event Action Canceled;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.UI.IsWorldInputBlocked == true || _isInteractionBlocked == true)
        {
            CancelInput();
            return;
        }

        UpdateInput();
    }

    private void OnDisable()
    {
        CancelInput();
    }

    public void SetInteractionBlocked(bool isBlocked)
    {
        _isInteractionBlocked = isBlocked;

        if (isBlocked == true)
        {
            CancelInput();
        }
    }

    public Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        return _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));
    }

    private void UpdateInput()
    {
        // 모바일
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;

            if (touch.press.wasPressedThisFrame == true)
            {
                BeginInput(touch.position.ReadValue(), true);
                return;
            }

            if (_isPointerPressed == true && _isTouchInput == true)
            {
                Vector2 touchPosition = touch.position.ReadValue();

                if (touch.press.wasReleasedThisFrame == true)
                    EndInput(touchPosition);
                else if (touch.press.isPressed == true)
                    MoveInput(touchPosition);

                return;
            }
        }

        if (Mouse.current == null)
            return;

        // PC
        if (Mouse.current.leftButton.wasPressedThisFrame == true)
        {
            BeginInput(Mouse.current.position.ReadValue(), false);
        }
        else if (_isPointerPressed == true && _isTouchInput == false)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasReleasedThisFrame == true)
                EndInput(mousePosition);
            else if (Mouse.current.leftButton.isPressed == true)
                MoveInput(mousePosition);
        }
    }

    private void BeginInput(Vector2 pointerPosition, bool isTouchInput)
    {
        _startPointerPosition = pointerPosition;
        _isPointerPressed = true;
        _isDragging = false;
        _isTouchInput = isTouchInput;
    }

    private void MoveInput(Vector2 pointerPosition)
    {
        if (_isDragging == false)
        {
            if (IsDragDistance(pointerPosition) == false)
                return;

            _isDragging = true;
            DragStarted?.Invoke(_startPointerPosition);
        }

        Dragged?.Invoke(pointerPosition);
    }

    private void EndInput(Vector2 pointerPosition)
    {
        if (_isDragging == false && IsDragDistance(pointerPosition) == true)
        {
            _isDragging = true;
            DragStarted?.Invoke(_startPointerPosition);
        }

        bool wasDragging = _isDragging;

        _isPointerPressed = false;
        _isDragging = false;

        if (wasDragging == true)
            DragEnded?.Invoke(pointerPosition);
        else
            Tapped?.Invoke(pointerPosition);
    }

    private bool IsDragDistance(Vector2 pointerPosition)
    {
        float tapDistance = Mathf.Min(Screen.width, Screen.height) * _tapDistanceRate;
        return (pointerPosition - _startPointerPosition).sqrMagnitude > tapDistance * tapDistance;
    }

    private void CancelInput()
    {
        if (_isPointerPressed == false)
            return;

        bool wasDragging = _isDragging;

        _isPointerPressed = false;
        _isDragging = false;

        if (wasDragging == true)
        {
            Canceled?.Invoke();
        }
    }
}
