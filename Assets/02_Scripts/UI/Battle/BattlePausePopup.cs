using UnityEngine;

public class BattlePausePopup : UIBase
{
    [SerializeField] private UIButtonComponent _restartButton;
    [SerializeField] private UIButtonComponent _exitButton;
    [SerializeField] private UIButtonComponent _returnButton;

    private void OnEnable()
    {
        GameManager.Time.Pause();

        _restartButton.BindButtonEvent(HandleRestartButton);
        _exitButton.BindButtonEvent(HandleExitButton);
        _returnButton.BindButtonEvent(HandleReturnButton);
    }

    private void OnDisable()
    {
        GameManager.Time.Resume();

        _restartButton.UnBindButtonAllEvent();
        _exitButton.UnBindButtonAllEvent();
        _returnButton.UnBindButtonAllEvent();
    }

    private void HandleRestartButton()
    {
        GameManager.Battle.RestartBattle();
        GameManager.UI.CloseBattlePausePopup();
    }

    private void HandleExitButton()
    {
        GameManager.Battle.EnterBattle();
        GameManager.UI.CloseBattlePausePopup();
    }

    private void HandleReturnButton()
    {
        GameManager.UI.CloseBattlePausePopup();
    }
}
