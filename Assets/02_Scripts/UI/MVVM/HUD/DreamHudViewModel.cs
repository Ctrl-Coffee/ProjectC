

public class DreamHudViewModel : ViewModelBase
{
    private UIBase _currentContent;

    public bool ExistCurrentContent => _currentContent != null;
    public bool IsOpenInventory => _currentContent is HeroInventoryView;

    public void OnChangeSceenToReal()
    {
        GameManager.Instance.ExitDream();
        GameManager.Instance.EnterReal();
    }

    public void ClearCurrentContent()
    {
        _currentContent.CloseUI();
        _currentContent = null;
    }

    public void OnOpenCompanion()
    {
        CompanionInventoryView content = GameManager.UI.OpenCompanionInventory();
        CloseCurrentContent(content);
    }

    public void OnOpenHeroInventory()
    {
        HeroInventoryView content = GameManager.UI.OpenHeroInventory();
        CloseCurrentContent(content);
    }

    public void OnOpenGacha()
    {
        GachaView content = GameManager.UI.OpenGachaView();
        CloseCurrentContent(content);
    }

    public void OnOpenHeroInfo()
    {
        HeroInfoView content = GameManager.UI.OpenHeroInfo();
        //CloseCurrentContent(content);
    }

    private void CloseCurrentContent(UIBase content)
    {
        if (content == null)
            return;

        if (_currentContent != null && _currentContent != content)
        {
            _currentContent.CloseUI();
        }

        _currentContent = content;
    }


    public override void InitializeModel() { }
    public override void UnBind() { }
}
