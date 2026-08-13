using TMPro;
using UnityEngine;

public class RealLobbyUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _money;
    [SerializeField] private TextMeshProUGUI _energy;
    [SerializeField] private TextMeshProUGUI _dreamPoint;
    [SerializeField] private TextMeshProUGUI _inspiration;

    [SerializeField] private UIButtonComponent _settingBtn;
    [SerializeField] private UIButtonComponent _goDreamBtn;
    [SerializeField] private UIButtonComponent _goDowntownBtn;

    [SerializeField] private UIButtonComponent _coffeeBtn;
    [SerializeField] private UIButtonComponent _computerBtn;

    private void OnEnable()
    {
        _computerBtn.BindButtonEvent(OnOpenWorkInfoUI);
    }

    private void OnOpenWorkInfoUI()
    {
        GameManager.UI.OpenWorkInfoUI();
    }
}
