using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SwipComponent : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _swipeTime = 0.2f;
    [SerializeField] private float _swipeDistanceRate = 0.2f;
    [FormerlySerializedAs("swipeDirect")]
    [SerializeField] private SwipeDirect _swipeDirect = SwipeDirect.Horizontal;

    private Vector3[] _pagePositions;

    private Vector2 _startTouchPosition;
    private Vector3 _startTouchWorldPosition;
    private Vector3 _dragStartPosition;

    private int _currentPage;
    private int _maxPage;

    private bool _isSwiping;
    private bool _isDragging;
    private bool _isTouchInput;

    private Tween _swipeTween;

    private void Awake()
    {
        if (_camera == null)
        {
            Logger.LogError("드래그 좌표를 변환할 Camera가 필요합니다.");
            enabled = false;
            return;
        }

        SetPagePositions();
    }

    private void Update()
    {
        UpdateInput();
    }

    private void OnDisable()
    {
        _swipeTween?.Kill();
        _isSwiping = false;
        _isDragging = false;
    }

    private void SetPagePositions()
    {
        int pageCount = transform.childCount;

        if (pageCount == 0)
        {
            Logger.LogError("스와이프할 자식 스프라이트가 없습니다.");
            enabled = false;
            return;
        }

        _pagePositions = new Vector3[pageCount];
        _pagePositions[0] = transform.position;

        Transform firstPage = transform.GetChild(0);
        SpriteRenderer firstRenderer = firstPage.GetComponent<SpriteRenderer>();

        if (firstRenderer == null)
        {
            Logger.LogError($"{firstPage.name}에 SpriteRenderer가 없습니다.");
            enabled = false;
            return;
        }

        float firstCenter = GetAxisValue(firstRenderer.bounds.center);
        float nextPagePosition = firstCenter + (GetAxisValue(firstRenderer.bounds.size) * 0.5f);

        for (int i = 1; i < pageCount; i++)
        {
            Transform page = transform.GetChild(i);
            SpriteRenderer renderer = page.GetComponent<SpriteRenderer>();

            if (renderer == null)
            {
                Logger.LogError($"{page.name}에 SpriteRenderer가 없습니다.");
                enabled = false;
                return;
            }

            float halfSize = GetAxisValue(renderer.bounds.size) * 0.5f;
            float targetCenter = nextPagePosition + halfSize;
            float positionOffset = targetCenter - GetAxisValue(renderer.bounds.center);

            page.position += GetAxisVector(positionOffset);
            _pagePositions[i] = _pagePositions[0] + GetAxisVector(firstCenter - targetCenter);

            nextPagePosition = targetCenter + halfSize;
        }

        _currentPage = 0;
        _maxPage = pageCount - 1;
    }

    private void UpdateInput()
    {
        if (_isSwiping == true)
            return;

        if (Touchscreen.current != null)
        {
            if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame == true)
            {
                _isTouchInput = true;
                BeginDrag(Touchscreen.current.primaryTouch.position.ReadValue());
                return;
            }

            if (_isDragging == true && _isTouchInput == true)
            {
                Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

                if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame == true)
                    EndDrag(touchPosition);
                else
                    Drag(touchPosition);

                return;
            }
        }

        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame == true)
        {
            _isTouchInput = false;
            BeginDrag(Mouse.current.position.ReadValue());
        }
        else if (_isDragging == true && _isTouchInput == false)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();

            if (Mouse.current.leftButton.wasReleasedThisFrame == true)
                EndDrag(mousePosition);
            else if (Mouse.current.leftButton.isPressed == true)
                Drag(mousePosition);
        }
    }

    private void BeginDrag(Vector2 touchPosition)
    {
        _startTouchPosition = touchPosition;
        _startTouchWorldPosition = GetWorldPosition(touchPosition);
        _dragStartPosition = transform.position;
        _isDragging = true;
    }

    private void Drag(Vector2 touchPosition)
    {
        float dragDistance = GetAxisValue(GetWorldPosition(touchPosition) - _startTouchWorldPosition);
        float targetPosition = GetAxisValue(_dragStartPosition) + dragDistance;
        float firstPagePosition = GetAxisValue(_pagePositions[0]);
        float lastPagePosition = GetAxisValue(_pagePositions[_maxPage]);

        targetPosition = Mathf.Clamp(
            targetPosition,
            Mathf.Min(firstPagePosition, lastPagePosition),
            Mathf.Max(firstPagePosition, lastPagePosition));

        transform.position = _dragStartPosition + GetAxisVector(targetPosition - GetAxisValue(_dragStartPosition));
    }

    private void EndDrag(Vector2 touchPosition)
    {
        Drag(touchPosition);
        _isDragging = false;

        float swipeDistance = GetAxisValue(touchPosition - _startTouchPosition);
        float screenSize = _swipeDirect == SwipeDirect.Horizontal ? Screen.width : Screen.height;

        if (Mathf.Abs(swipeDistance) < screenSize * _swipeDistanceRate)
        {
            Swipe(_currentPage);
            return;
        }

        float nextPageDirection = _maxPage == 0 ? 0f : Mathf.Sign(GetAxisValue(_pagePositions[_maxPage] - _pagePositions[0]));

        if (Mathf.Sign(swipeDistance) == nextPageDirection)
        {
            _currentPage = Mathf.Min(_currentPage + 1, _maxPage);
        }
        else
        {
            _currentPage = Mathf.Max(_currentPage - 1, 0);
        }

        Swipe(_currentPage);
    }

    private void Swipe(int index)
    {
        _swipeTween?.Kill();

        _isSwiping = true;
        _swipeTween = transform.DOMove(_pagePositions[index], _swipeTime)
            .OnComplete(() => _isSwiping = false);
    }

    private float GetAxisValue(Vector2 value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? value.x : value.y;
    }

    private float GetAxisValue(Vector3 value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? value.x : value.y;
    }

    private Vector3 GetWorldPosition(Vector2 screenPosition)
    {
        float cameraDistance = Vector3.Dot(transform.position - _camera.transform.position, _camera.transform.forward);

        return _camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, cameraDistance));
    }

    private Vector3 GetAxisVector(float value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? new Vector3(value, 0f, 0f) : new Vector3(0f, value, 0f);
    }
}
