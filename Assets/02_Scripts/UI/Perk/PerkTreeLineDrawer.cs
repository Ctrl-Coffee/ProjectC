using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkTreeLineDrawer : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private RectTransform _lineRoot;
    [SerializeField] private RectTransform _nodeRoot;

    [Header("선 모양")]
    [SerializeField] private float _lineWidth = 6f;
    [SerializeField] private Color _lineColor = new Color(0.42f, 0.44f, 0.5f, 1f);
    [SerializeField] private Color _activeLineColor = new Color(1f, 0.85f, 0.4f, 1f);

    private struct PerkLine
    {
        public Image Image;
        public string ParentId;
        public string ChildId;
    }

    private Dictionary<string, PerkNodeUI> _nodeMap = new();
    private List<PerkNodeUI> _nodeBuffer = new();

    private List<PerkLine> _lines = new();
    private bool _isBuilt = false;

    public RectTransform NodeRoot
    {
        get { return _nodeRoot; }
    }

    public void Refresh()
    {
        if (!_isBuilt)
            BuildLines(null);

        RefreshLineColors();
    }

    public void Rebuild()
    {
        ClearLines();
        Refresh();
    }

    public void RebuildWithTable(IReadOnlyDictionary<string, PerkNodeData> table)
    {
        ClearLines();
        BuildLines(table);
        RefreshLineColors();
    }

    private PerkNodeData GetNodeData(IReadOnlyDictionary<string, PerkNodeData> table, string nodeId)
    {
        if (null != table)
        {
            return table.TryGetValue(nodeId, out PerkNodeData data) ? data : null;
        }

        return GameManager.DataTable.GetPerkNodeData(nodeId);
    }

    private void BuildLines(IReadOnlyDictionary<string, PerkNodeData> table)
    {
        if (!ValidateReferences()) 
            return;

        if (!BuildNodeMap()) 
            return;

        foreach (KeyValuePair<string, PerkNodeUI> pair in _nodeMap)
        {
            PerkNodeData data = GetNodeData(table, pair.Key);

            if (null == data)
            {
                Logger.LogWarning($"테이블에 없는 노드가 배치돼 있습니다. id: {pair.Key}");
                continue;
            }

            string[] parentIds = data.ParentId;

            if (null == parentIds)
                continue;

            for (int i = 0; i < parentIds.Length; i++)
            {
                if (!_nodeMap.TryGetValue(parentIds[i], out PerkNodeUI parentNode))
                {
                    Logger.LogWarning($"선행 노드가 배치돼 있지 않습니다. {pair.Key} <- {parentIds[i]}");
                    continue;
                }

                CreateLine(parentIds[i], pair.Key, parentNode.RectTransform, pair.Value.RectTransform);
            }
        }

        _isBuilt = true;

        Logger.Log($"퍽 트리 연결선 {_lines.Count}개를 생성했습니다. (노드 {_nodeMap.Count}개)");
    }

    private void RefreshLineColors()
    {
        bool hasPerkState = Application.isPlaying && null != GameManager.Instance;

        for (int i = 0; i < _lines.Count; i++)
        {
            PerkLine line = _lines[i];

            if (null == line.Image)
                continue;

            bool isActive = hasPerkState
                && GameManager.Perk.IsUnlocked(line.ParentId)
                && GameManager.Perk.IsUnlocked(line.ChildId);

            line.Image.color = isActive ? _activeLineColor : _lineColor;
        }
    }

    private void ClearLines()
    {
        _lines.Clear();
        _isBuilt = false;

        if (null == _lineRoot)
            return;

        for (int i = _lineRoot.childCount - 1; i >= 0; i--)
        {
            GameObject lineObject = _lineRoot.GetChild(i).gameObject;

            if (Application.isPlaying)
            {
                Destroy(lineObject);
            }
            else
            {
                DestroyImmediate(lineObject);
            }
        }
    }

    private bool ValidateReferences()
    {
        if (null == _lineRoot)
        {
            Logger.LogError("LineRoot 가 지정되지 않았습니다.");
            return false;
        }

        if (null == _nodeRoot)
        {
            Logger.LogError("NodeRoot 가 지정되지 않았습니다.");
            return false;
        }

        return true;
    }

    private bool BuildNodeMap()
    {
        _nodeMap.Clear();
        _nodeBuffer.Clear();

        _nodeRoot.GetComponentsInChildren<PerkNodeUI>(true, _nodeBuffer);

        for (int i = 0; i < _nodeBuffer.Count; i++)
        {
            PerkNodeUI node = _nodeBuffer[i];
            string nodeId = node.NodeId;

            if (_nodeMap.ContainsKey(nodeId))
            {
                Logger.LogError($"배치된 노드 Id 가 중복됐습니다. id: {nodeId}");
                continue;
            }

            _nodeMap.Add(nodeId, node);
        }

        if (_nodeMap.Count == 0)
        {
            Logger.LogWarning("NodeRoot 아래에 PerkNodeUI 가 하나도 없습니다.");
            return false;
        }

        return true;
    }

    private void CreateLine(string fromId, string toId, RectTransform from, RectTransform to)
    {
        Vector2 fromLocal = _lineRoot.InverseTransformPoint(from.position);
        Vector2 toLocal = _lineRoot.InverseTransformPoint(to.position);
        Vector2 diff = toLocal - fromLocal;

        GameObject lineObject = new GameObject($"Line_{fromId}__{toId}", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        lineObject.layer = _lineRoot.gameObject.layer;

        RectTransform lineRect = lineObject.transform as RectTransform;
        lineRect.SetParent(_lineRoot, false);

        lineRect.anchorMin = new Vector2(0.5f, 0.5f);
        lineRect.anchorMax = new Vector2(0.5f, 0.5f);
        lineRect.pivot = new Vector2(0f, 0.5f);
        lineRect.localScale = Vector3.one;
        lineRect.localPosition = fromLocal;
        lineRect.sizeDelta = new Vector2(diff.magnitude, _lineWidth);
        lineRect.localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg);

        Image lineImage = lineObject.GetComponent<Image>();
        lineImage.color = _lineColor;
        lineImage.raycastTarget = false;

        PerkLine line = new PerkLine();
        line.Image = lineImage;
        line.ParentId = fromId;
        line.ChildId = toId;

        _lines.Add(line);
    }
}
