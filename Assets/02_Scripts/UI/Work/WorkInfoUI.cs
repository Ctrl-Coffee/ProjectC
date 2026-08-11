using UnityEngine;

public class WorkInfoUI : UIBase
{
    [SerializeField] private UIButtonComponent _btnStart;
    [SerializeField] private UIButtonComponent _btnClose;

    private WorkFlowHandler _workHandler = new();

    private void OnEnable()
    {
        if (null == _btnStart || null == _btnClose)
        {
            Logger.LogError("Button이 연결되지 않았습니다.");
            return;
        }

        _btnStart.BindButtonEvent(OnClickStartMiniGame);
        _btnClose.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        if (null == _btnStart || null == _btnClose)
        {
            return;
        }

        _btnStart.UnBindAllButtonEvent();
        _btnClose.UnBindAllButtonEvent();
    }

    private void OnDestroy()
    {
        _workHandler.Cancel();
    }

    private void OnClickStartMiniGame()
    {
        _workHandler.StartMiniGameAsync().Forget();
    }
}
