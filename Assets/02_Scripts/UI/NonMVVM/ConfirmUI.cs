using TMPro;

public class ConfirmUI : UIBase
{
    public TextMeshProUGUI _titleTxet;
    public TextMeshProUGUI _descTxet;

    public UIButtonComponent _okButtonn;
    public UIButtonComponent _cancelButtonn;

    public void SetConfirmUI(ConfirmData confirmData, ConfirmButtonAction buttonAction)
    {
        _titleTxet.text = confirmData.TitleTxet;
        _descTxet.text = confirmData.DescTxet;

        _okButtonn.ChangeButtonText(confirmData.OKText);
        _cancelButtonn.ChangeButtonText(confirmData.CancelText);

        _okButtonn.BindButtonEvent(buttonAction.OnClickOKButton);
        _okButtonn.BindButtonEvent(OnCloseUI);

        _cancelButtonn.BindButtonEvent(buttonAction.OnClickCancelButton);
        _cancelButtonn.BindButtonEvent(OnCloseUI);

        _okButtonn.gameObject.SetActive(true);
        _cancelButtonn.gameObject.SetActive(Utils.ParseEnum<ConfirmType>(confirmData.ConfirmType) == ConfirmType.OK_CANCEL);
    }

    private void OnCloseUI()
    {
        CloseUI();
    }
}