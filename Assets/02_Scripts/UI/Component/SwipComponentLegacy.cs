using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SwipComponentLegacy : MonoBehaviour
{
    [SerializeField] private Scrollbar _scrollbar;
    [SerializeField] private float _swipeTime = 0.2f;
    [SerializeField] private SwipeDirect _swipeDirect = SwipeDirect.Horizontal;

    private float _minSwipeDistance;

    private float[] _scrollPageStartValues;
    private float _pageWidth;

    private float _startTouch;
    private float _endTouch;

    private int _currentPage;
    private int _maxPage;

    private bool _isSwiping;

    private Tween _swipeTween;

    private void Awake()
    {
        _scrollPageStartValues = new float[transform.childCount];

        _pageWidth = 1f / (transform.childCount - 1);

        for (int i = 0; i < _scrollPageStartValues.Length; i++)
        {
            _scrollPageStartValues[i] = i * _pageWidth;
        }

        _maxPage = transform.childCount - 1;

        _minSwipeDistance = Screen.width * 0.2f;
    }

    private void Start()
    {
        SetScrollBarValue(0);
    }

    private void Update()
    {
        UpdateInput();
    }

    private void SetScrollBarValue(int index)
    {
        _currentPage = index;
        _scrollbar.value = _scrollPageStartValues[index];
    }

    private void UpdateInput()
    {
        if (_isSwiping == true)
            return;

#if UNITY_EDITOR
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (_swipeDirect == SwipeDirect.Horizontal)
                _startTouch = Mouse.current.position.ReadValue().x;
            else
                _startTouch = Mouse.current.position.ReadValue().y;
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            if (_swipeDirect == SwipeDirect.Horizontal)
                _endTouch = Mouse.current.position.ReadValue().x;
            else
                _endTouch = Mouse.current.position.ReadValue().y;

            UpdateSwipe();
        }
#endif

#if UNITY_ANDROID
        if (Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            _startTouch = Touchscreen.current.primaryTouch.position.ReadValue().x;
        }
        else if (Touchscreen.current.primaryTouch.press.wasReleasedThisFrame)
        {
            _endTouch = Touchscreen.current.primaryTouch.position.ReadValue().x;
            UpdateSwipe();
        }
#endif
    }

    private void UpdateSwipe()
    {
        if (Mathf.Abs(_endTouch - _startTouch) < _minSwipeDistance)
        {
            Swipe(_currentPage);
            return;
        }

        bool isLeft = _startTouch < _endTouch;

        if (isLeft)
        {
            if (_currentPage == 0)
                return;

            _currentPage--;
        }
        else
        {
            if (_currentPage == _maxPage)
                return;

            _currentPage++;
        }

        Swipe(_currentPage);
    }

    private void Swipe(int index)
    {
        _swipeTween?.Kill();

        _isSwiping = true;

        _swipeTween = DOTween.To(
            () => _scrollbar.value,
            value => _scrollbar.value = value,
            _scrollPageStartValues[index],
            _swipeTime
        ).OnComplete(() => _isSwiping = false);
    }
}
