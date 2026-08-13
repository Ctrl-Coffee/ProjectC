using TMPro;
using UnityEngine;

public class DreamLobbyUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _dreamPoint;
    [SerializeField] private TextMeshProUGUI _fragmentDream;
    [SerializeField] private TextMeshProUGUI _scrollDream;
    [SerializeField] private TextMeshProUGUI _inspiration;

    [SerializeField] private UIButtonComponent _settingBtn;

    [SerializeField] private UIButtonComponent _forgeBtn;
    [SerializeField] private UIButtonComponent _guildBtn;
    [SerializeField] private UIButtonComponent _stageBtn;
    [SerializeField] private UIButtonComponent _heroBtn;
    [SerializeField] private UIButtonComponent _lobbyBtn;


    private void OnEnable()
    {
        _lobbyBtn.BindButtonEvent(OnChangeSceenToReal);
    }

    private void OnDisable()
    {
        _lobbyBtn.UnBindButtonAllEvent();
    }

    private void OnChangeSceenToReal()
    {
        GameManager.Instance.ExitDream();
        GameManager.Instance.EnterReal();
    }
}
