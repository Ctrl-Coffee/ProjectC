using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Dictionary<Type, UIBase> _createdUI = new();
    private Stack<Type> _openedUI = new();

    private List<Transform> _canvasLayer;

    public async UniTask Init()
    {
        if (_canvasLayer == null)
        {
            await CreateUIRoot();
        }
    }

    public async UniTask<T> OpenUI<T>(UIRootType uiRootType) where T : UIBase
    {
        Type uiType = typeof(T);

        if (_openedUI.Contains(uiType))
        {
            Logger.Log($"{uiType}가 이미 열려있습니다");
            return null;
        }

        var ui = await GetOrCreateUI<T>(uiRootType);

        if (!ui)
        {
            Logger.Log($"{uiType}가 null 입니다.");
            return null;
        }

        _openedUI.Push(uiType);

        ui.gameObject.SetActive(true);
        ui.OpenUI();

        return ui;
    }

    public void CloseUI(UIBase ui, bool isCloseAll = false)
    {
        var uiType = ui.GetType();

        if (_openedUI.Peek() != uiType)
        {
            return;
        }

        _openedUI.Pop();

        if (isCloseAll == true)
        {
            ui.transform.DOKill();
            ui.gameObject.SetActive(false);
            return;
        }

        ui.PlayCloseAnimation().OnComplete(() =>
        {
            ui.gameObject.SetActive(false);
        });
    }

    public UIBase GetCurrentFrontUI()
    {
        if (_openedUI.Count == 0)
        {
            return null;
        }

        return _createdUI[_openedUI.Peek()];
    }

    public UniTask<T> OpenHUDUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Hud);
    }

    public UniTask<T> OpenPopupUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Popup);
    }

    private async UniTask<T> GetOrCreateUI<T>(UIRootType uiRootType) where T : UIBase
    {
        var uiType = typeof(T);

        T ui = null;

        if (_createdUI.ContainsKey(uiType))
        {
            ui = _createdUI[uiType] as T;
            return ui;
        }

        var uiPrefab = await GameManager.Resource.LoadAssetAsync<GameObject>(AddressablePath.GetUIPath(uiType));

        if (uiPrefab == null)
        {
            Logger.LogError($"{uiType.Name}가 null입니다.");
            return null;
        }

        ui = GameObject.Instantiate(uiPrefab, _canvasLayer[(int)uiRootType]).GetComponent<UIBase>() as T;
        _createdUI.Add(uiType, ui);

        return ui;
    }

    private async UniTask CreateUIRoot()
    {
        GameObject uiRootPrefab = await GameManager.Resource.LoadAssetAsync<GameObject>(AddressablePath.Prefab.UIRoot);

        if (uiRootPrefab == null)
        {
            Logger.LogError("UIRoot 프리팹이 null입니다.");
            return;
        }

        var uiRootInstance = GameObject.Instantiate(uiRootPrefab);

        _canvasLayer = new(uiRootInstance.GetComponent<UIManagerHelper>().Canvas);
    }
}
