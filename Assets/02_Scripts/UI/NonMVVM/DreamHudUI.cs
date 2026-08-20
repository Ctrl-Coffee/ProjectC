using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;

public class DreamHudUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _dreamPoint;
    [SerializeField] private TextMeshProUGUI _fragmentDream;
    [SerializeField] private TextMeshProUGUI _scrollDream;
    [SerializeField] private TextMeshProUGUI _inspiration;

    [SerializeField] private UIButtonComponent _settingBtn;

    [SerializeField] private UIButtonComponent _gachaBtn;
    [SerializeField] private UIButtonComponent _companionBtn;
    [SerializeField] private UIButtonComponent _stageBtn;
    [SerializeField] private UIButtonComponent _heroBtn;
    [SerializeField] private UIButtonComponent _lobbyBtn;

    private UIBase _currentContent;

    private void OnEnable()
    {
        _companionBtn.BindButtonEvent(OnOpenCompanion);
        _stageBtn.BindButtonEvent(OnStage);
        _heroBtn.BindButtonEvent(OnOpenHero);
        _lobbyBtn.BindButtonEvent(OnChangeSceenToReal);
    }

    private void OnDisable()
    {
        _companionBtn.UnBindButtonAllEvent();
        _stageBtn.UnBindButtonAllEvent();
        _heroBtn.UnBindButtonAllEvent();
        _lobbyBtn.UnBindButtonAllEvent();
    }

    private void OnChangeSceenToReal()
    {
        GameManager.Instance.ExitDream();
        GameManager.Instance.EnterReal();
    }

    private void OnStage()
    {
        if (_currentContent == null)
            return;

        _currentContent.CloseUI();
        _currentContent = null;
    }

    private async void OnOpenCompanion()
    {
        var content = await GameManager.UI.OpenCompanionInventory();

        CloseCurrentContent(content);
    }


    private async void OnOpenHero()
    {
        var content = await GameManager.UI.OpenHeroInventory();

        CloseCurrentContent(content);
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
}
