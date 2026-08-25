using TMPro;
using UnityEngine;

public class RealHudUI : UIBase
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
    [SerializeField] private UIButtonComponent _perkBtn;

    private void OnEnable()
    {
        _settingBtn.BindButtonEvent(OnOpenSettingUI);

        _goDreamBtn.BindButtonEvent(OnChangeSceenToDream);

        _computerBtn.BindButtonEvent(OnOpenWorkInfoUI);
        _perkBtn.BindButtonEvent(OnOpenPerkInfoUI);
    }

    private void OnDisable()
    {
        _settingBtn.UnBindButtonAllEvent();
        _goDreamBtn.UnBindButtonAllEvent();

        _coffeeBtn.UnBindButtonAllEvent();
        _computerBtn.UnBindButtonAllEvent();
        _perkBtn.UnBindButtonAllEvent();
    }

    private void OnOpenSettingUI()
    {
        GameManager.UI.OpenSettingUI();
    }

    private void OnChangeSceenToDream()
    {
        GameManager.Instance.ExitReal();
        GameManager.Instance.EnterDream();
    }

    private void OnOpenWorkInfoUI()
    {
        GameManager.UI.OpenWorkInfoUI();
    }

    private void OnOpenPerkInfoUI()
    {
        GameManager.UI.OpenPerkInfoUI();
    }

    private void OKAction()
    {
        Debug.Log("OKAction");
    }

    private void CancleAction()
    {
        Debug.Log("CancleAction");
    }
}
