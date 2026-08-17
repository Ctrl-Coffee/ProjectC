using System.Collections.Generic;
using UnityEngine;

public class PerkInfoUI : UIBase
{
    [SerializeField] private UIButtonComponent _btnClose;
    [SerializeField] private PerkTreeLineDrawer _lineDrawer;

    private List<PerkNodeUI> _nodes = new();
    private bool _isBound = false;

    public PerkTreeLineDrawer LineDrawer
    {
        get
        {
            return _lineDrawer;
        }
    }

    private void OnEnable()
    {
        BindNodes();
        RefreshAll();

        if (null == _btnClose)
        {
            Logger.LogError("_btnClose 가 연결되지 않았습니다.");
            return;
        }

        _btnClose.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        if (null == _btnClose)
        {
            return;
        }

        _btnClose.UnBindButtonAllEvent();
    }

    public void RefreshAll()
    {
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].Refresh();
        }

        if (null != _lineDrawer)
        {
            _lineDrawer.Refresh();
        }
    }

    public void OnClickNode(string perkId)
    {
        GameManager.UI.OpenPerkDetailUI(perkId, this);
    }

    private void BindNodes()
    {
        if (_isBound)
        {
            return;
        }

        if (null == _lineDrawer || null == _lineDrawer.NodeRoot)
        {
            Logger.LogError("LineDrawer 또는 NodeRoot 가 지정되지 않았습니다.");
            return;
        }

        _isBound = true;

        _lineDrawer.NodeRoot.GetComponentsInChildren<PerkNodeUI>(true, _nodes);

        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodes[i].Bind(this);
        }
    }
}
