using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.UI;

public class WorkQueueSlotUI : MonoBehaviour
{
    [SerializeField] private Image _imgIcon;
    [SerializeField] private Image _imgFill;
    [SerializeField] private UIButtonComponent _btnSlot;

    private int _index;
    private Action<int> _onClick;
    private string _iconKey = string.Empty;

    public void Bind(int index, Action<int> onClick)
    {
        _index = index;
        _onClick = onClick;

        if (null == _btnSlot)
        {
            Logger.LogError($"큐 슬롯 {index}번의 버튼이 연결되지 않아 취소할 수 없습니다.");
            return;
        }

        _btnSlot.UnBindButtonAllEvent();
        _btnSlot.BindButtonEvent(OnClickSlot);
    }

    public void Unbind()
    {
        _onClick = null;

        if (null == _btnSlot)
        {
            return;
        }

        _btnSlot.UnBindButtonAllEvent();
    }

    public void SetIcon(string iconKey)
    {
        if (_iconKey == iconKey)
        {
            return;
        }

        _iconKey = iconKey;

        if (Utils.IsNullOrWhiteSpace(iconKey))
        {
            HideIcon();
            return;
        }

        LoadIconAsync(iconKey).Forget();
    }

    private async UniTaskVoid LoadIconAsync(string iconKey)
    {
        Sprite sprite;

        try
        {
            sprite = await GameManager.Resource.LoadAssetAsync<Sprite>(iconKey, this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException)
        {
            return;
        }
        catch (Exception)
        {
            return;
        }

        if (_iconKey != iconKey)
        {
            return;
        }

        if (null == sprite || null == _imgIcon)
        {
            return;
        }

        _imgIcon.sprite = sprite;
        _imgIcon.enabled = true;
    }

    private void HideIcon()
    {
        if (null == _imgIcon)
        {
            return;
        }

        _imgIcon.sprite = null;
        _imgIcon.enabled = false;
    }

    public void SetEmpty()
    {
        _iconKey = string.Empty;
        HideIcon();

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
