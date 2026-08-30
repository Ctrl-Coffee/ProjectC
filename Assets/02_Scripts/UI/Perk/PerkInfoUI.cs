using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkInfoUI : UIBase
{
    [SerializeField] private UIButtonComponent _btnClose;
    [SerializeField] private PerkTreeLineDrawer _lineDrawer;
    [SerializeField] private Button _btnPerkStat;

    [Header("노드 아이콘")]
    [SerializeField] private Sprite _minorIcon;
    [SerializeField] private Sprite _notableIcon;
    [SerializeField] private Sprite _unlockIcon;
    [SerializeField] private Sprite _keystoneIcon;

    private List<PerkNodeUI> _nodes = new();
    private bool _isBound = false;
    private PerkStatView _perkStatView;

    public PerkTreeLineDrawer LineDrawer
    {
        get
        {
            return _lineDrawer;
        }
    }

    private void Awake()
    {
        if (null == _btnClose)
        {
            Logger.LogError("_btnClose 가 연결되지 않았습니다.");
            return;
        }

        _btnClose.BindButtonEvent(OnClickCloseButton);

        if (null != _btnPerkStat)
        {
            _btnPerkStat.onClick.AddListener(OnClickPerkStat);
        }
    }

    private void OnClickPerkStat()
    {
        _perkStatView = GameManager.UI.OpenPerkStat();
    }

    private void ClosePerkStat()
    {
        if (null == _perkStatView)
        {
            return;
        }

        _perkStatView.CloseUI();
        _perkStatView = null;
    }

    private void OnEnable()
    {
        BindNodes();
        RefreshAll();

        GameManager.Perk.OnPerkChanged += RefreshAll;
    }

    private void OnDisable()
    {
        GameManager.Perk.OnPerkChanged -= RefreshAll;

        ClosePerkStat();
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
        GameManager.UI.OpenPerkDetailUI(perkId);
    }

    public Sprite GetNodeIcon(PerkNodeType nodeType)
    {
        switch (nodeType)
        {
            case PerkNodeType.Minor:
                return _minorIcon;

            case PerkNodeType.Notable:
                return _notableIcon;

            case PerkNodeType.Unlock:
                return _unlockIcon;

            case PerkNodeType.Keystone:
                return _keystoneIcon;
        }

        return null;
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
