using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class PerkTreeToolWindow : EditorWindow
{
    private const string TABLE_PATH = "JsonOutput/PerkNodeData";
    private const string NODE_PREFAB_PATH = "Assets/03_Prefabs/UI/Perk/PerkNode.prefab";
    private const string PARENT_MODE_NONE = "None";

    private const float CHILD_OFFSET_X = 220f;
    private const float CHILD_OFFSET_Y = 160f;

    private const int REPORT_LIMIT = 20;

    [Serializable]
    private class SerializationWrapper
    {
        public List<PerkNodeData> items;
    }

    private Dictionary<string, PerkNodeData> _table = new();
    private GameObject _nodePrefab;
    private Vector2 _scroll;

    [MenuItem("Tools/Perk Tree Tool")]
    private static void Open()
    {
        GetWindow<PerkTreeToolWindow>("Perk Tree");
    }

    private void OnEnable()
    {
        LoadTable();

        if (null == _nodePrefab)
        {
            _nodePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(NODE_PREFAB_PATH);
        }
    }

    private void OnGUI()
    {
        _scroll = EditorGUILayout.BeginScrollView(_scroll);

        DrawSetup();
        EditorGUILayout.Space();

        PerkTreeLineDrawer drawer = GetDrawer();

        if (null == drawer)
        {
            EditorGUILayout.HelpBox(
                "PerkTreeLineDrawer 를 찾을 수 없습니다.\n" +
                "PerkInfoUI 는 씬에 없는 프리팹이라, Project 창에서 PerkInfoUI.prefab 을 더블클릭해 프리팹 모드로 열어야 합니다.\n" +
                "(플레이 중이라면 씬에 생성된 인스턴스를 자동으로 찾습니다.)",
                MessageType.Warning);
            EditorGUILayout.EndScrollView();
            return;
        }

        if (null == drawer.NodeRoot)
        {
            EditorGUILayout.HelpBox("LineDrawer 의 NodeRoot 가 지정되지 않았습니다.", MessageType.Error);
            EditorGUILayout.EndScrollView();
            return;
        }

        Dictionary<string, PerkNodeUI> placed = CollectPlacedNodes(drawer.NodeRoot);

        DrawValidation(placed);
        EditorGUILayout.Space();

        DrawActions(drawer, placed);

        EditorGUILayout.EndScrollView();
    }

    #region 설정

    private void DrawSetup()
    {
        EditorGUILayout.LabelField("설정", EditorStyles.boldLabel);

        _nodePrefab = EditorGUILayout.ObjectField("노드 프리팹", _nodePrefab, typeof(GameObject), false) as GameObject;

        EditorGUILayout.LabelField("테이블 노드", $"{_table.Count}개");

        if (GUILayout.Button("테이블 다시 읽기"))
        {
            LoadTable();
        }
    }

    private void LoadTable()
    {
        _table.Clear();

        TextAsset textAsset = Resources.Load<TextAsset>(TABLE_PATH);

        if (null == textAsset)
        {
            Debug.LogError($"퍽 테이블을 찾을 수 없습니다. Resources/{TABLE_PATH}");
            return;
        }

        SerializationWrapper wrapper = JsonUtility.FromJson<SerializationWrapper>("{\"items\":" + textAsset.text + "}");

        if (null == wrapper || null == wrapper.items)
        {
            Debug.LogError("퍽 테이블 파싱에 실패했습니다.");
            return;
        }

        for (int i = 0; i < wrapper.items.Count; i++)
        {
            PerkNodeData data = wrapper.items[i];

            if (string.IsNullOrEmpty(data.Id))
            {
                continue;
            }

            if (_table.ContainsKey(data.Id))
            {
                Debug.LogError($"테이블에 중복된 Id 가 있습니다. id: {data.Id}");
                continue;
            }

            _table.Add(data.Id, data);
        }
    }

    private PerkTreeLineDrawer GetDrawer()
    {
        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

        if (null != stage)
        {
            return stage.prefabContentsRoot.GetComponentInChildren<PerkTreeLineDrawer>(true);
        }

        return FindFirstObjectByType<PerkTreeLineDrawer>(FindObjectsInactive.Include);
    }

    private Dictionary<string, PerkNodeUI> CollectPlacedNodes(RectTransform nodeRoot)
    {
        Dictionary<string, PerkNodeUI> placed = new();

        PerkNodeUI[] nodes = nodeRoot.GetComponentsInChildren<PerkNodeUI>(true);

        for (int i = 0; i < nodes.Length; i++)
        {
            string nodeId = nodes[i].NodeId;

            if (string.IsNullOrEmpty(nodeId) || placed.ContainsKey(nodeId))
            {
                continue;
            }

            placed.Add(nodeId, nodes[i]);
        }

        return placed;
    }

    #endregion

    #region 검증

    private void DrawValidation(Dictionary<string, PerkNodeUI> placed)
    {
        EditorGUILayout.LabelField("검증", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("배치된 노드", $"{placed.Count}개");

        List<string> missingNodes = FindMissingNodes(placed);
        List<string> orphanNodes = FindOrphanNodes(placed);
        List<string> missingParents = FindMissingParents();
        List<string> brokenLinks = FindBrokenLinks(placed);

        DrawReport("미배치 노드 (테이블에만 있음)", missingNodes);
        DrawReport("정체불명 노드 (테이블에 없음)", orphanNodes);
        DrawReport("존재하지 않는 부모 참조", missingParents);
        DrawReport("부모가 미배치라 선이 끊김", brokenLinks);

        if (0 == missingNodes.Count && 0 == orphanNodes.Count && 0 == missingParents.Count && 0 == brokenLinks.Count)
        {
            EditorGUILayout.HelpBox("문제 없음", MessageType.Info);
        }
    }

    private List<string> FindMissingNodes(Dictionary<string, PerkNodeUI> placed)
    {
        List<string> result = new();

        foreach (KeyValuePair<string, PerkNodeData> pair in _table)
        {
            if (!placed.ContainsKey(pair.Key))
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }

    private List<string> FindOrphanNodes(Dictionary<string, PerkNodeUI> placed)
    {
        List<string> result = new();

        foreach (KeyValuePair<string, PerkNodeUI> pair in placed)
        {
            if (!_table.ContainsKey(pair.Key))
            {
                result.Add(pair.Key);
            }
        }

        return result;
    }

    private List<string> FindMissingParents()
    {
        List<string> result = new();

        foreach (KeyValuePair<string, PerkNodeData> pair in _table)
        {
            string[] parentIds = pair.Value.ParentId;

            if (null == parentIds)
            {
                continue;
            }

            for (int i = 0; i < parentIds.Length; i++)
            {
                if (string.IsNullOrEmpty(parentIds[i]))
                {
                    continue;
                }

                if (!_table.ContainsKey(parentIds[i]))
                {
                    result.Add($"{pair.Key} -> {parentIds[i]}");
                }
            }
        }

        return result;
    }

    private List<string> FindBrokenLinks(Dictionary<string, PerkNodeUI> placed)
    {
        List<string> result = new();

        foreach (KeyValuePair<string, PerkNodeUI> pair in placed)
        {
            if (!_table.TryGetValue(pair.Key, out PerkNodeData data) || null == data.ParentId)
            {
                continue;
            }

            for (int i = 0; i < data.ParentId.Length; i++)
            {
                string parentId = data.ParentId[i];

                if (string.IsNullOrEmpty(parentId) || !_table.ContainsKey(parentId))
                {
                    continue;
                }

                if (!placed.ContainsKey(parentId))
                {
                    result.Add($"{pair.Key} <- {parentId}");
                }
            }
        }

        return result;
    }

    private void DrawReport(string label, List<string> items)
    {
        if (0 == items.Count)
        {
            EditorGUILayout.LabelField(label, "0개");
            return;
        }

        EditorGUILayout.LabelField(label, $"{items.Count}개");

        EditorGUI.indentLevel++;

        int count = Mathf.Min(items.Count, REPORT_LIMIT);

        for (int i = 0; i < count; i++)
        {
            EditorGUILayout.LabelField($"    {items[i]}");
        }

        if (items.Count > count)
        {
            EditorGUILayout.LabelField($"    ... 외 {items.Count - count}개");
        }

        EditorGUI.indentLevel--;
    }

    #endregion

    #region 작업

    private void DrawActions(PerkTreeLineDrawer drawer, Dictionary<string, PerkNodeUI> placed)
    {
        EditorGUILayout.LabelField("작업", EditorStyles.boldLabel);

        EditorGUI.BeginDisabledGroup(null == _nodePrefab);

        if (GUILayout.Button("누락 노드 생성"))
        {
            CreateMissingNodes(drawer.NodeRoot, placed);
        }

        EditorGUI.EndDisabledGroup();

        if (null == _nodePrefab)
        {
            EditorGUILayout.HelpBox("노드 프리팹을 지정해야 생성할 수 있습니다.", MessageType.Info);
        }

        if (GUILayout.Button("연결선 다시 만들기"))
        {
            Undo.RegisterFullObjectHierarchyUndo(drawer.gameObject, "Rebuild Perk Lines");

            drawer.RebuildWithTable(_table);

            MarkDirty(drawer.gameObject);
        }
    }

    private void CreateMissingNodes(RectTransform nodeRoot, Dictionary<string, PerkNodeUI> placed)
    {
        List<string> missing = FindMissingNodes(placed);

        if (0 == missing.Count)
        {
            return;
        }

        Dictionary<string, int> childCounts = new();
        int createdCount = 0;
        bool isCreated = true;

        while (isCreated)
        {
            isCreated = false;

            for (int i = missing.Count - 1; i >= 0; i--)
            {
                PerkNodeData data = _table[missing[i]];

                if (!TryCreateNode(data, nodeRoot, placed, childCounts))
                {
                    continue;
                }

                missing.RemoveAt(i);
                createdCount++;
                isCreated = true;
            }
        }

        for (int i = 0; i < missing.Count; i++)
        {
            CreateNode(_table[missing[i]], nodeRoot, Vector2.zero, placed, childCounts);
            createdCount++;
        }

        MarkDirty(nodeRoot.gameObject);

        Debug.Log($"누락 노드 {createdCount}개를 생성했습니다.");
    }

    private bool TryCreateNode(PerkNodeData data, RectTransform nodeRoot, Dictionary<string, PerkNodeUI> placed, Dictionary<string, int> childCounts)
    {
        PerkNodeUI parent = FindPlacedParent(data, placed);

        if (null == parent)
        {
            return false;
        }

        Vector2 basePosition = parent.RectTransform.anchoredPosition;

        CreateNode(data, nodeRoot, basePosition, placed, childCounts, parent.NodeId);

        return true;
    }

    private PerkNodeUI FindPlacedParent(PerkNodeData data, Dictionary<string, PerkNodeUI> placed)
    {
        if (null == data.ParentId || PARENT_MODE_NONE == data.ParentMode)
        {
            return null;
        }

        for (int i = 0; i < data.ParentId.Length; i++)
        {
            if (string.IsNullOrEmpty(data.ParentId[i]))
            {
                continue;
            }

            if (placed.TryGetValue(data.ParentId[i], out PerkNodeUI parent))
            {
                return parent;
            }
        }

        return null;
    }

    private void CreateNode(PerkNodeData data, RectTransform nodeRoot, Vector2 basePosition, Dictionary<string, PerkNodeUI> placed, Dictionary<string, int> childCounts, string parentId = "")
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(_nodePrefab, nodeRoot) as GameObject;

        if (null == instance)
        {
            Debug.LogError($"노드 프리팹을 생성하지 못했습니다. id: {data.Id}");
            return;
        }

        Undo.RegisterCreatedObjectUndo(instance, "Create Perk Node");

        instance.name = data.Id;

        RectTransform rect = instance.transform as RectTransform;

        if (null != rect)
        {
            rect.anchoredPosition = CalcPosition(basePosition, parentId, childCounts);
            rect.localScale = Vector3.one;
        }

        PerkNodeUI node = instance.GetComponent<PerkNodeUI>();

        if (null == node)
        {
            Debug.LogError($"노드 프리팹에 PerkNodeUI 가 없습니다. id: {data.Id}");
            return;
        }

        SetNodeId(node, data.Id);

        placed.Add(data.Id, node);
    }

    private Vector2 CalcPosition(Vector2 basePosition, string parentId, Dictionary<string, int> childCounts)
    {
        string key = string.IsNullOrEmpty(parentId) ? "__root" : parentId;

        childCounts.TryGetValue(key, out int index);
        childCounts[key] = index + 1;

        float offsetX = string.IsNullOrEmpty(parentId) ? 0f : CHILD_OFFSET_X;

        return basePosition + new Vector2(offsetX, -CHILD_OFFSET_Y * index);
    }

    private void SetNodeId(PerkNodeUI node, string nodeId)
    {
        SerializedObject serialized = new SerializedObject(node);
        SerializedProperty property = serialized.FindProperty("_nodeId");

        if (null == property)
        {
            Debug.LogError("PerkNodeUI 에 _nodeId 필드를 찾을 수 없습니다.");
            return;
        }

        property.stringValue = nodeId;
        serialized.ApplyModifiedProperties();
    }

    private void MarkDirty(GameObject target)
    {
        PrefabStage stage = PrefabStageUtility.GetCurrentPrefabStage();

        if (null != stage)
        {
            EditorSceneManager.MarkSceneDirty(stage.scene);
            return;
        }

        EditorUtility.SetDirty(target);
        EditorSceneManager.MarkSceneDirty(target.scene);
    }

    #endregion
}
