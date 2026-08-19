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

    private void OnEnable()
    {
        _settingBtn.BindButtonEvent(OnOpenSettingUI);

        _goDreamBtn.BindButtonEvent(OnChangeSceenToDream);

        _coffeeBtn.BindButtonEvent(TEST);
        _computerBtn.BindButtonEvent(OnOpenWorkInfoUI);
    }

    private void OnDisable()
    {
        _settingBtn.UnBindButtonAllEvent();
        _goDreamBtn.UnBindButtonAllEvent();

        _coffeeBtn.UnBindButtonAllEvent();
        _computerBtn.UnBindButtonAllEvent();
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

    private void TEST()
    {
        var confirmData = GameManager.DataTable.GetConfirmData(ConfirmDataKey.TEST_CONFIRM);
        var buttonAction = new ConfirmButtonAction();
        buttonAction.OnClickOKButton = OKAction;
        buttonAction.OnClickCancelButton = CancleAction;

        GameManager.UI.OpenConfirmUI(confirmData, buttonAction);
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
