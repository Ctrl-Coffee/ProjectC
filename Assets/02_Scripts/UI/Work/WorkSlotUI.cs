using System;
using TMPro;
using UnityEngine;

public class WorkSlotUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtName;
    [SerializeField] private TextMeshProUGUI _txtInfo;
    [SerializeField] private UIButtonComponent _btnPlay;

    private string _workId;
    private Action<string> _onClickPlay;

    public void Bind(string workId, Action<string> onClickPlay)
    {
        _workId = workId;
        _onClickPlay = onClickPlay;

        if (null == _btnPlay)
        {
            Logger.LogError("WorkSlot의 버튼이 연결되지 않았습니다.");
            return;
        }

        _btnPlay.UnBindAllButtonEvent();
        _btnPlay.BindButtonEvent(OnClickPlay);
    }

    public void Unbind()
    {
        _onClickPlay = null;

        if (null == _btnPlay)
        {
            return;
        }

        _btnPlay.UnBindAllButtonEvent();
    }

    public void SetInfo(string workName, string info)
    {
        if (null != _txtName)
        {
            _txtName.text = workName;
        }

        if (null != _txtInfo)
        {
            _txtInfo.text = info;
        }
    }

    private void OnClickPlay()
    {
        if (null == _onClickPlay)
        {
            return;
        }

        _onClickPlay(_workId);
    }
}
