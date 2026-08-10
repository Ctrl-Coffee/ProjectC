using UnityEngine;
using System.Collections.Generic;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class UIManager
{
    private Dictionary<Type, UIBase> _openUIs = new();
    private List<UIBase> _openUIOrder = new();

    private Dictionary<Type, UIBase> _createdUIs = new();

    private Transform _uiRoot;

    public async UniTask Init()
    {
        if(_uiRoot == null)
        {
            await CreateUIRoot();
        }
    }

    public T OpenUI<T>() where T : UIBase
    {
        Type uiType = typeof(T);
        T ui = GetOrCreateUI<T>(out var isAlreadyOpen);

        if (!ui)
        {
            Logger.Log($"{uiType}가 null 입니다.");
            return null;
        }

        if (isAlreadyOpen)
        {
            Logger.Log($"{uiType}가 이미 열려있습니다");
            return null;
        }

        _openUIOrder.Add(ui);
        _openUIs.Add(typeof(T), ui);

        ui.transform.SetAsLastSibling();
        ui.gameObject.SetActive(true);
        ui.OpenUI();

        return ui;
    }

    public void CloseUI(UIBase ui, bool isCloseAll = false)
    {
        var uiType = ui.GetType();

        if (!_openUIs.Remove(uiType))
        {
            return;
        }

        _openUIOrder.Remove(ui);

        if(isCloseAll == true)
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

    public void CloseAllOpenUI() 
    {
        while (_openUIOrder.Count > 0)
        {
            CloseUI(_openUIOrder[_openUIOrder.Count - 1], true);
        }
    }

    public bool ExistOpenUI() 
    { 
        return _openUIs.Count > 0;
    }

    public UIBase GetCurrentFrontUI() 
    { 
        if(_openUIOrder.Count == 0)
        {
            return null;
        }

        return _openUIOrder[_openUIOrder.Count - 1];
    }

    public void CloseCurrentFrontUI()
    {
        if (_openUIOrder.Count == 0)
        {
            return;
        }

        CloseUI(_openUIOrder[_openUIOrder.Count - 1]);
    }

    private async UniTask CreateUIRoot()
    {
        GameObject uiRootPrefab = await GameManager.Resource.LoadAssetAsync<GameObject>(AddressablePath.Prefab.UIRoot);

        if(uiRootPrefab == null)
        {
            Logger.LogError("UIRoot 프리팹이 null입니다.");
            return;
        }

        var uiRootInstance = GameObject.Instantiate(uiRootPrefab);
        _uiRoot = uiRootInstance.transform;
    }

    private T GetOrCreateUI<T>(out bool isAlreadyOpen) where T : UIBase
    {
        var uiType = typeof(T);

        T ui = null;
        isAlreadyOpen = false;

        if (_openUIs.ContainsKey(uiType))
        {
            isAlreadyOpen = true;
            ui = _openUIs[uiType] as T;
        }
        else if(_createdUIs.ContainsKey(uiType))
        {
            ui = _createdUIs[uiType] as T;
        }
        else
        {
            var uiPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(uiType));
            
            if (uiPrefab == null)
            {
                Logger.LogError($"{uiType.Name}가 null입니다.");
                return null;
            }

            ui = GameObject.Instantiate(uiPrefab, _uiRoot).GetComponent<UIBase>() as T;
            _createdUIs.Add(uiType, ui);
        }
            
        return ui;
    }
}
