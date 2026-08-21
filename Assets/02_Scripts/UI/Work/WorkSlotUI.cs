using Cysharp.Threading.Tasks;
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

        _btnPlay.UnBindButtonAllEvent();
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
            Logger.LogError("WorkSlot의 썸네일 Image가 연결되지 않았습니다.");
            return;
        }

        if (Utils.IsNullOrWhiteSpace(iconKey))
        {
            return;
        }

        LoadIconAsync(iconKey, _workId).Forget();
    }

    private async UniTaskVoid LoadIconAsync(string iconKey, string requestedWorkId)
    {
        Sprite sprite;

        try
        {
            sprite = await GameManager.Resource.LoadAssetAsync<Sprite>(iconKey, destroyCancellationToken);
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception exception)
        {
            Logger.LogWarning($"업무 아이콘 로드에 실패했습니다. key: {iconKey}, {exception.Message}");
            return;
        }

        if (requestedWorkId != _workId)
        {
            return;
        }

        if (null == sprite)
        {
            Logger.LogWarning($"업무 아이콘을 불러오지 못했습니다. key: {iconKey}");
            return;
        }

        if (null == _imgThumbnail)
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
