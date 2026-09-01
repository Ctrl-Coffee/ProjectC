using UnityEngine;

public class BattlePauseUI : UIBase
{
    [SerializeField] private UIButtonComponent _returnButton;

    private void OnEnable()
    {
        GameManager.Time.Pause();
        _returnButton.BindButtonEvent(HandleReturnButton);
    }

    private void OnDisable()
    {
        GameManager.Time.Resume();
        _returnButton.UnBindButtonAllEvent();
    }

    private void HandleReturnButton()
    {
        GameManager.UI.CloseBattlePauseUI();
    }
}
