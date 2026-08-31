using UnityEngine;

public class TestBackground : MonoBehaviour
{
    [SerializeField] private Transform[] _backgrounds;
    [SerializeField] private float _scrollSpeed = 2f;
    [SerializeField] private float _backgroundWidth = 20f;

    private bool _isScrolling;

    public void StartScroll()
    {
        _isScrolling = true;
    }

    public void StopScroll()
    {
        _isScrolling = false;
    }


    private void Start()
    {
        _isScrolling = true;
    }

    private void Update()
    {
        if (!_isScrolling)
        {
            return;
        }

        float movement = _scrollSpeed * Time.deltaTime;

        foreach (Transform background in _backgrounds)
        {
            background.position += Vector3.left * movement;

            if (background.position.x <= -_backgroundWidth)
            {
                MoveToEnd(background);
            }
        }
    }

    private void MoveToEnd(Transform background)
    {
        float rightmostX = float.MinValue;

        foreach (Transform other in _backgrounds)
        {
            if (other == background)
            {
                continue;
            }

            rightmostX = Mathf.Max(
                rightmostX,
                other.position.x);
        }

        Vector3 position = background.position;
        position.x = rightmostX + _backgroundWidth;

        background.position = position;
    }
}
