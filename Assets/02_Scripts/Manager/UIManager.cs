using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
    private Dictionary<Type, UIBase> _createdUI = new();
    private Dictionary<Type, UIState> _uiStates = new();
    private Dictionary<Type, UIRootType> _uiRootTypes = new();
    private Stack<Type> _popupStack = new();

    private List<Transform> _canvasLayer;

    private int _openedUICount = 0;

    public bool IsWorldInputBlocked => _openedUICount > 0;

    public void Init()
    {
        if (_canvasLayer == null)
        {
            CreateUIRoot();
        }
    }

    public T OpenUI<T>(UIRootType uiRootType) where T : UIBase
    {
        Type uiType = typeof(T);

        if (_uiStates.TryGetValue(uiType, out UIState state) && state != UIState.Closed)
        {
            return null;
        }

        if (_uiRootTypes.ContainsKey(uiType) && _uiRootTypes[uiType] != uiRootType)
        {
            Logger.LogError($"{uiType}은 {uiRootType}이 아닙니다.");
            return null;
        }

        _uiStates[uiType] = UIState.Opening;

        try
        {
            T ui = GetOrCreateUI<T>(uiRootType);

            if (null == ui)
            {
                Logger.Log($"{uiType}가 null 입니다.");
                _uiStates[uiType] = UIState.Closed;
                return null;
            }

            ui.gameObject.SetActive(true);

            if (uiRootType == UIRootType.Popup)
            {
                ui.transform.SetAsLastSibling();
                _popupStack.Push(uiType);
            }

            if(uiRootType != UIRootType.Hud)
            {
                _openedUICount++;
            }

            _uiStates[uiType] = UIState.Opened;

            ui.OpenUI();
            return ui;
        }
        catch
        {
            _uiStates[uiType] = UIState.Closed;
            throw;
        }
    }

    public void CloseUI(UIBase ui, bool isCloseAll = false)
    {
        var uiType = ui.GetType();

        if (!_uiStates.TryGetValue(uiType, out UIState state) || state != UIState.Opened)
        {
            return;
        }

        if (_uiRootTypes[uiType] == UIRootType.Popup)
        {
            if (_popupStack.Count == 0 || _popupStack.Peek() != uiType)
            {
                return;
            }
        }

        _uiStates[uiType] = UIState.Closing;

        if (isCloseAll == true)
        {
            ui.transform.DOKill();
            CompleteClose(ui);
            return;
        }

        if(ui.IsPlayAnimation == true)
        {
            ui.PlayCloseAnimation().OnComplete(() =>
            {
                CompleteClose(ui);
            });
        }
        else
        {
            CompleteClose(ui);
        }   
    }

    public void CloseAllPopups()
    {
        while (_popupStack.Count > 0)
        {
            var uiType = _popupStack.Peek();
            if (_createdUI.TryGetValue(uiType, out UIBase ui))
            {
                CloseUI(ui, true);
            }
            else
            {
                _popupStack.Pop();
            }
        }
    }

    public UIBase GetCurrentFrontPopupUI()
    {
        if (_popupStack.Count == 0)
        {
            return null;
        }

        return _createdUI[_popupStack.Peek()];
    }

    public T OpenContentUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Content);
    }

    public T OpenHUDUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Hud);
    }

    public T OpenPopupUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Popup);
    }

    public T OpenOverlayUI<T>() where T : UIBase
    {
        return OpenUI<T>(UIRootType.Overlay);
    }

    public T CloseContentUI<T>() where T : UIBase
    {
        T ui = GetOrCreateUI<T>(UIRootType.Content);
        ui.CloseUI();
        return ui;
    }

    public T ClosePopupUI<T>() where T : UIBase
    {
        T ui = GetOrCreateUI<T>(UIRootType.Popup);
        ui.CloseUI();
        return ui;
    }

    public T CloseHUDUI<T>() where T : UIBase
    {
        T ui = GetOrCreateUI<T>(UIRootType.Hud);
        ui.CloseUI();
        return ui;
    }

    private T GetOrCreateUI<T>(UIRootType uiRootType) where T : UIBase
    {
        var uiType = typeof(T);

        T ui = null;

        if (_createdUI.ContainsKey(uiType))
        {
            ui = _createdUI[uiType] as T;
            return ui;
        }

        var uiPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(uiType));

        if (uiPrefab == null)
        {
            Logger.LogError($"{uiType.Name}가 null입니다.");
            return null;
        }

        if (!uiPrefab.TryGetComponent<T>(out T uiComponent))
        {
            Logger.LogError($"{uiType.Name} 프리팹에 {uiType.Name} 컴포넌트가 없습니다.");
            return null;
        }

        ui = GameObject.Instantiate(uiComponent, _canvasLayer[(int)uiRootType]);

        _createdUI.Add(uiType, ui);
        _uiRootTypes[uiType] = uiRootType;

        return ui;
    }

    private void CreateUIRoot()
    {
        GameObject uiRootPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.UIROOT);

        if (uiRootPrefab == null)
        {
            Logger.LogError("UIRoot 프리팹이 null입니다.");
            return;
        }

        var uiRootInstance = GameObject.Instantiate(uiRootPrefab);

        _canvasLayer = new(uiRootInstance.GetComponent<UIManagerHelper>().Canvas);
    }

    private void CompleteClose(UIBase ui)
    {
        var uiType = ui.GetType();

        if (_uiRootTypes[uiType] != UIRootType.Hud)
        {
            _openedUICount--;
        }

        _uiStates[uiType] = UIState.Closed;

        if (_uiRootTypes.ContainsKey(uiType) && _uiRootTypes[uiType] == UIRootType.Popup)
        {
            _popupStack.Pop();
        }

        ui.gameObject.SetActive(false);
    }

    private void ClearUIRootType()
    {
        _uiRootTypes.Clear();
    }

    private void ClearCreatedUI()
    {
        foreach (var ui in _createdUI.Values)
        {
            GameObject.Destroy(ui.gameObject);
        }

        _createdUI.Clear();
        _uiStates.Clear();
    }
}
