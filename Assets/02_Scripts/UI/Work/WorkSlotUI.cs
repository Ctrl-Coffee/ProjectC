using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorkSlotUI : MonoBehaviour
{
    [SerializeField] private Image _imgThumbnail;
    [SerializeField] private TextMeshProUGUI _txtName;
    [SerializeField] private TextMeshProUGUI _txtDesc;
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

        _btnPlay.BindButtonEvent(OnClickPlay);
    }

    public void Unbind()
    {
        _onClickPlay = null;

        if (null == _btnPlay)
        {
            return;
        }

        _btnPlay.UnBindButtonAllEvent();
    }

    public void SetInfo(string workName, string desc)
    {
        if (null != _txtName)
        {
            _txtName.text = workName;
        }

        if (null != _txtDesc)
        {
            _txtDesc.text = desc;
        }
    }

    public void SetIcon(string iconKey)
    {
        if (null == _imgThumbnail)
        {
            return;
        }

        if (Utils.IsNullOrWhiteSpace(iconKey))
        {
            return;
        }

        LoadIcon(iconKey);
    }

    private void LoadIcon(string iconKey)
    {
        Sprite sprite = GameManager.Resource.GetLoadedAsset<Sprite>(iconKey);

        if (null == sprite || null == _imgThumbnail)
        {
            return;
        }

        _imgThumbnail.sprite = sprite;
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
