using DG.Tweening;
using System;
using UnityEngine;

public class SwipComponent : MonoBehaviour
{
    public event Action<int> OnPageChanged;

    [SerializeField] private BackgroundInputHandler _inputHandler; 
    [SerializeField] private Transform _worldContent;

    [Header("Swipe Settings")]
    [SerializeField] private SwipeDirect _swipeDirect = SwipeDirect.Horizontal;
    [SerializeField] private float _swipeDistanceRate = 0.2f;
    [SerializeField] private float _swipeTime = 0.2f;

    [Header("Page")]
    [SerializeField] private int _startPage = 0;

    private Vector3[] _pagePositions;

    private Vector2 _startTouchPosition;
    private Vector3 _startTouchWorldPosition;
    private Vector3 _dragStartPosition;

    private int _currentPage;
    private int _maxPage;

    private bool _isDragging;

    private Tween _swipeTween;

    private void Awake()
    {
        SetPagePositions();
        SetPage(_startPage, true);
    }

    private void OnEnable()
    {
        _inputHandler.OnDragStarted += BeginDrag;
        _inputHandler.OnDragged += Drag;
        _inputHandler.OnDragEnded += EndDrag;
        _inputHandler.OnCanceled += CancelDrag;
    }

    private void OnDisable()
    {
        _inputHandler.OnDragStarted -= BeginDrag;
        _inputHandler.OnDragged -= Drag;
        _inputHandler.OnDragEnded -= EndDrag;
        _inputHandler.OnCanceled -= CancelDrag;

        _swipeTween?.Kill();
        _inputHandler.SetInteractionBlocked(false);
        _isDragging = false;
    }

    public void SetPage(int index, bool immediately = false)
    {
        int pageIndex = Mathf.Clamp(index, 0, _maxPage);
        Swipe(pageIndex, immediately);
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
        _pagePositions[0] = _worldContent.position;

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

    private void BeginDrag(Vector2 touchPosition)
    {
        _startTouchPosition = touchPosition;
        _startTouchWorldPosition = _inputHandler.GetWorldPosition(touchPosition);
        _dragStartPosition = _worldContent.position;
        _isDragging = true;
    }

    private void Drag(Vector2 touchPosition)
    {
        float dragDistance = GetAxisValue(_inputHandler.GetWorldPosition(touchPosition) - _startTouchWorldPosition);
        float targetPosition = GetAxisValue(_dragStartPosition) + dragDistance;
        float firstPagePosition = GetAxisValue(_pagePositions[0]);
        float lastPagePosition = GetAxisValue(_pagePositions[_maxPage]);

        targetPosition = Mathf.Clamp(
            targetPosition,
            Mathf.Min(firstPagePosition, lastPagePosition),
            Mathf.Max(firstPagePosition, lastPagePosition));

        _worldContent.position = _dragStartPosition + GetAxisVector(targetPosition - GetAxisValue(_dragStartPosition));
    }

    private void EndDrag(Vector2 touchPosition)
    {
        Drag(touchPosition);
        _isDragging = false;

        float swipeDistance = GetAxisValue(touchPosition - _startTouchPosition);
        float swipeScreenSize = _swipeDirect == SwipeDirect.Horizontal ? Screen.width : Screen.height;

        if (Mathf.Abs(swipeDistance) < swipeScreenSize * _swipeDistanceRate)
        {
            Swipe(_currentPage);
            return;
        }

        int previousPage = _currentPage;

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

        if (_currentPage != previousPage)
        {
            OnPageChanged?.Invoke(_currentPage);
        }
    }

    private void CancelDrag()
    {
        if (_isDragging == false)
            return;

        _isDragging = false;
        Swipe(_currentPage);
    }

    private void Swipe(int index, bool immediately = false)
    {
        _swipeTween?.Kill();
        _inputHandler.SetInteractionBlocked(false);
        _currentPage = index;

        if (immediately == true)
        {
            _worldContent.position = _pagePositions[index];
            return;
        }

        _inputHandler.SetInteractionBlocked(true);
        _swipeTween = _worldContent.DOMove(_pagePositions[index], _swipeTime)
            .OnComplete(() => _inputHandler.SetInteractionBlocked(false));
    }

    private float GetAxisValue(Vector2 value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? value.x : value.y;
    }

    private float GetAxisValue(Vector3 value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? value.x : value.y;
    }

    private Vector3 GetAxisVector(float value)
    {
        return _swipeDirect == SwipeDirect.Horizontal ? new Vector3(value, 0f, 0f) : new Vector3(0f, value, 0f);
    }
}
