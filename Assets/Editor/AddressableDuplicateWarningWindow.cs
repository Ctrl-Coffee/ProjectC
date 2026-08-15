using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

[InitializeOnLoad]
internal static class AddressableDuplicatePlayModeValidator
{
    private const string _settingsPath = "Assets/00_ThirdParty/AddressableAssetsData/AddressableAssetSettings.asset";

    private static bool _skipNextValidation;

    static AddressableDuplicatePlayModeValidator()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    internal static void ContinuePlayMode()
    {
        _skipNextValidation = true;
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state != PlayModeStateChange.ExitingEditMode)
        {
            return;
        }

        if (_skipNextValidation)
        {
            _skipNextValidation = false;
            return;
        }

        List<AddressableDuplicateIssue> issues = CollectIssues();
        if (issues.Count == 0)
        {
            return;
        }

        EditorApplication.isPlaying = false;
        EditorApplication.delayCall += () => AddressableDuplicateWarningWindow.Open(issues);
    }

    private static List<AddressableDuplicateIssue> CollectIssues()
    {
        AddressableAssetSettings settings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>(_settingsPath);
        if (settings == null)
        {
            return new List<AddressableDuplicateIssue>();
        }

        Dictionary<string, AddressableDuplicateIssue> issuesByGuid = new Dictionary<string, AddressableDuplicateIssue>();

        foreach (AddressableAssetGroup group in settings.groups)
        {
            if (group == null)
            {
                continue;
            }

            foreach (AddressableAssetEntry entry in group.entries)
            {
                if (!issuesByGuid.TryGetValue(entry.guid, out AddressableDuplicateIssue issue))
                {
                    issue = new AddressableDuplicateIssue(entry.guid, entry.address);
                    issuesByGuid.Add(entry.guid, issue);
                }

                issue.AddEntry(group.Name, entry.labels);
            }
        }

        return issuesByGuid.Values
            .Where(issue => issue.HasMultipleGroups || issue.HasMultipleLabels)
            .OrderBy(issue => issue.AssetPath)
            .ToList();
    }
}

internal sealed class AddressableDuplicateIssue
{
    private readonly string _guid;
    private readonly string _address;
    private readonly List<string> _groupNames = new List<string>();
    private readonly List<string> _multipleLabelDescriptions = new List<string>();

    internal string Address => _address;
    internal string AssetPath => AssetDatabase.GUIDToAssetPath(_guid);
    internal bool HasMultipleGroups => _groupNames.Count > 1;
    internal bool HasMultipleLabels => _multipleLabelDescriptions.Count > 0;
    internal string GroupNames => string.Join(", ", _groupNames);
    internal string MultipleLabelDescriptions => string.Join(" / ", _multipleLabelDescriptions);

    internal AddressableDuplicateIssue(string guid, string address)
    {
        _guid = guid;
        _address = address;
    }

    internal void AddEntry(string groupName, HashSet<string> labels)
    {
        if (!_groupNames.Contains(groupName))
        {
            _groupNames.Add(groupName);
        }

        if (labels.Count > 1)
        {
            string labelNames = string.Join(", ", labels.OrderBy(label => label));
            _multipleLabelDescriptions.Add($"{groupName}: {labelNames}");
        }
    }
}

internal sealed class AddressableDuplicateWarningWindow : EditorWindow
{
    private const string _windowTitle = "Addressables 설정 경고";

    private List<AddressableDuplicateIssue> _issues;
    private Vector2 _scrollPosition;

    internal static void Open(List<AddressableDuplicateIssue> issues)
    {
        AddressableDuplicateWarningWindow window = CreateInstance<AddressableDuplicateWarningWindow>();
        window._issues = issues;
        window.titleContent = new GUIContent(_windowTitle);
        window.minSize = new Vector2(620f, 360f);
        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField("Addressables 중복 설정이 발견되었습니다.", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            $"중복 조건에 해당하는 에셋 {_issues.Count}개가 있어 Play Mode 진입을 중단했습니다. " +
            "설정을 확인하거나 이번 실행에 한해 경고를 무시할 수 있습니다.",
            MessageType.Warning);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        foreach (AddressableDuplicateIssue issue in _issues)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(issue.AssetPath, EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Address", issue.Address);

            if (issue.HasMultipleGroups)
            {
                EditorGUILayout.LabelField("여러 그룹", issue.GroupNames);
            }

            if (issue.HasMultipleLabels)
            {
                EditorGUILayout.LabelField("여러 라벨", issue.MultipleLabelDescriptions);
            }

            EditorGUILayout.EndVertical();
        }

        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space(6f);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("닫기", GUILayout.Width(100f)))
        {
            Close();
        }

        if (GUILayout.Button("이번만 무시하고 실행", GUILayout.Width(170f)))
        {
            Close();
            AddressableDuplicatePlayModeValidator.ContinuePlayMode();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(8f);
    }
}
