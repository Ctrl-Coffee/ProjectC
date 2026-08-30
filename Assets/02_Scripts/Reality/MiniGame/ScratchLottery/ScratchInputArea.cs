using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScratchInputArea : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [SerializeField] private float _stepDistance = 6f;

    public event Action<Vector2> OnScratch;

    private Vector2 _previousPosition;
    private bool _hasPrevious;

    private void OnDisable()
    {
        _hasPrevious = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _previousPosition = eventData.position;
        _hasPrevious = true;

        OnScratch?.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 currentPosition = eventData.position;

        if (_hasPrevious == false)
        {
            _previousPosition = currentPosition;
            _hasPrevious = true;
        }

        float distance = Vector2.Distance(_previousPosition, currentPosition);
        int stepCount = Mathf.CeilToInt(distance / _stepDistance);

        for (int step = 1; step <= stepCount; step++)
        {
            float ratio = step / (float)stepCount;

            OnScratch?.Invoke(Vector2.Lerp(_previousPosition, currentPosition, ratio));
        }

        _previousPosition = currentPosition;
    }


}
