using System;
using UnityEngine;
using UnityEngine.UI;

public class WorkQueueSlotUI : MonoBehaviour
{
    [SerializeField] private Image _imgFill;
    [SerializeField] private UIButtonComponent _btnSlot;

    private int _index;
    private Action<int> _onClick;

    public void Bind(int index, Action<int> onClick)
    {
        _index = index;
        _onClick = onClick;

        if (null == _btnSlot)
        {
            Logger.LogError($"큐 슬롯 {index}번의 버튼이 연결되지 않아 취소할 수 없습니다.");
            return;
        }

        _btnSlot.UnBindAllButtonEvent();
        _btnSlot.BindButtonEvent(OnClickSlot);
    }

    public void Unbind()
    {
        _onClick = null;

        if (null == _btnSlot)
        {
            return;
        }

        _btnSlot.UnBindAllButtonEvent();
    }

    public void SetEmpty()
    {
        if (null == _imgFill)
        {
            return;
        }

        _imgFill.fillAmount = 0f;
    }

    public void SetProgress(float progress)
    {
        if (null == _imgFill)
        {
            return;
        }

        _imgFill.fillAmount = progress;
    }

    private void OnClickSlot()
    {
        if (null == _onClick)
        {
            return;
        }

        _onClick(_index);
    }
}
