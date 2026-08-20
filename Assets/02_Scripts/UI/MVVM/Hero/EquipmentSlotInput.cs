using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotInput : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerExitHandler
{
    private string _heroEquipmentId;

    private const float _longPressDuration = 0.5f;

    private event Action _onEquipEvent;
    private event Action<string> _onDetailEvent;

    private bool _isLongPressed;

    private PointerEventData _pressEventData;

    private CancellationTokenSource _token = new();

    public void Init(string heroEquipmentId, Action onEquip, Action<string> onDetail)
    {
        _heroEquipmentId = heroEquipmentId;
        _onEquipEvent = onEquip;
        _onDetailEvent = onDetail;
    }

    private void OnDisable()
    {
        CancelLongPress();
    }

    public void SetEquipmentId(string id)
    {
        _heroEquipmentId = id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_isLongPressed)
        {
            return;
        }

        _onEquipEvent?.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        CancelLongPress();

        _token = new CancellationTokenSource();

        _isLongPressed = false;
        _pressEventData = eventData;
        CheckLongPressAsync(_token.Token).Forget();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelLongPress();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelLongPress();
    }

    private async UniTask CheckLongPressAsync(CancellationToken token)
    {
        bool isCanceled = await UniTask
        .Delay(TimeSpan.FromSeconds(_longPressDuration), ignoreTimeScale: true, cancellationToken: token)
        .SuppressCancellationThrow();

        if (isCanceled || _pressEventData == null || _pressEventData.dragging)
        {
            return;
        }

        _isLongPressed = true;
        _onDetailEvent?.Invoke(_heroEquipmentId);
    }

    private void CancelLongPress()
    {
        if (_token != null)
        {
            _token.Cancel();
            _token.Dispose();
            _token = null;
        }

        _pressEventData = null;
    }
}
