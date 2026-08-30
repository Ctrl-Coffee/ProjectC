using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EquipmentDebugWindow : EditorWindow
{
    private const double REPAINT_INTERVAL = 0.25;
    private const float BUTTON_WIDTH = 52f;
    private const float STATE_WIDTH = 70f;

    private class EquipmentDataComparer : IComparer<EquipmentData>
    {
        public int Compare(EquipmentData left, EquipmentData right)
        {
            int typeCompare = GetTypeOrder(left).CompareTo(GetTypeOrder(right));

            if (0 != typeCompare)
            {
                return typeCompare;
            }

            int gradeCompare = left.EquipmentGrade.CompareTo(right.EquipmentGrade);

            if (0 != gradeCompare)
            {
                return gradeCompare;
            }

            return string.Compare(left.Id, right.Id, System.StringComparison.Ordinal);
        }

        private int GetTypeOrder(EquipmentData data)
        {
            return (int)Utils.ParseEnum<EquipmentType>(data.EquipmentTypeString);
        }
    }

    private static readonly EquipmentType[] EQUIPMENT_TYPES =
    {
        EquipmentType.Weapon,
        EquipmentType.Armor,
        EquipmentType.Accessory,
    };

    private static readonly EquipmentGrade[] EQUIPMENT_GRADES =
    {
        EquipmentGrade.Rare,
        EquipmentGrade.Epic,
        EquipmentGrade.Unique,
        EquipmentGrade.Legendary,
    };

    private static readonly string[] TYPE_FILTER_LABELS = { "전체", "무기", "방어구", "장신구" };
    private static readonly string[] GRADE_FILTER_LABELS = { "전체", "Rare", "Epic", "Unique", "Legendary" };

    private readonly List<EquipmentData> _sortedEquipments = new();
    private readonly EquipmentDataComparer _comparer = new();

    private int _typeFilterIndex;
    private int _gradeFilterIndex;
    private string _searchText = string.Empty;
    private bool _showOwnedOnly;

    private Vector2 _scrollPosition;
    private double _nextRepaintTime;

    [MenuItem("Tools/Equipment Debug")]
    private static void Open()
    {
        GetWindow<EquipmentDebugWindow>("Equipment Debug");
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (EditorApplication.timeSinceStartup < _nextRepaintTime)
        {
            return;
        }

        _nextRepaintTime = EditorApplication.timeSinceStartup + REPAINT_INTERVAL;

        Repaint();
    }

    private void OnDisable()
    {
        _sortedEquipments.Clear();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("장비 지급은 플레이 모드에서만 동작합니다.", MessageType.Info);
            _sortedEquipments.Clear();
            return;
        }

        if (null == GameManager.Instance)
        {
            EditorGUILayout.HelpBox("GameManager가 아직 준비되지 않았습니다.", MessageType.Warning);
            return;
        }

        HeroEquipmentModel equipment = GameManager.Session.HeroEquipment;
        HeroEquipedModel equiped = GameManager.Session.HeroEquiped;

        if (null == equipment || null == equiped)
        {
            EditorGUILayout.HelpBox("장비 데이터가 아직 준비되지 않았습니다.", MessageType.Warning);
            return;
        }

        EnsureSortedEquipments();

        DrawEquipedSlots(equiped, equipment);
        EditorGUILayout.Space();

        DrawBulkButtons(equipment, equiped);
        EditorGUILayout.Space();

        DrawFilter();
        EditorGUILayout.Space();

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        DrawEquipmentList(equipment, equiped);

        EditorGUILayout.EndScrollView();
    }

    private void EnsureSortedEquipments()
    {
        Dictionary<string, EquipmentData> table = GameManager.DataTable.EquipmentDataTable;

        if (_sortedEquipments.Count == table.Count)
        {
            return;
        }

        _sortedEquipments.Clear();

        foreach (KeyValuePair<string, EquipmentData> pair in table)
        {
            _sortedEquipments.Add(pair.Value);
        }

        _sortedEquipments.Sort(_comparer);
    }

    private void DrawEquipedSlots(HeroEquipedModel equiped, HeroEquipmentModel equipment)
    {
        EditorGUILayout.LabelField("착용 중", EditorStyles.boldLabel);

        for (int i = 0; i < EQUIPMENT_TYPES.Length; i++)
        {
            DrawEquipedSlotRow(equiped, equipment, EQUIPMENT_TYPES[i]);
        }
    }

    private void DrawEquipedSlotRow(HeroEquipedModel equiped, HeroEquipmentModel equipment, EquipmentType equipmentType)
    {
        string equipedId = equiped.GetEquipedId(equipmentType);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(GetTypeName(equipmentType), GetEquipedName(equipedId, equipment), GUILayout.MinWidth(120f));

        EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(equipedId));

        if (GUILayout.Button("해제", GUILayout.Width(BUTTON_WIDTH)))
        {
            equiped.UnEquip(equipmentType);
            Logger.Log($"[EquipmentDebug] {GetTypeName(equipmentType)} 해제");
        }

        EditorGUI.EndDisabledGroup();

        EditorGUILayout.EndHorizontal();
    }

    private string GetEquipedName(string equipedId, HeroEquipmentModel equipment)
    {
        if (string.IsNullOrEmpty(equipedId))
        {
            return "없음";
        }

        EquipmentData data = GameManager.DataTable.GetEquipmentData(equipedId);
        string displayName = null == data ? equipedId : data.Name;

        return $"{displayName} (Lv.{equipment.GetLevel(equipedId)})";
    }

    private void DrawBulkButtons(HeroEquipmentModel equipment, HeroEquipedModel equiped)
    {
        EditorGUILayout.LabelField("일괄", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("보이는 장비 전부 지급", GUILayout.Height(24f)))
        {
            GiveFiltered(equipment);
        }

        if (GUILayout.Button("보유 장비 전부 제거", GUILayout.Height(24f)))
        {
            RemoveAll(equipment, equiped);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void GiveFiltered(HeroEquipmentModel equipment)
    {
        int count = 0;

        for (int i = 0; i < _sortedEquipments.Count; i++)
        {
            EquipmentData data = _sortedEquipments[i];

            if (!IsVisible(data, equipment))
            {
                continue;
            }

            if (null != equipment.GetHeroEquipment(data.Id))
            {
                continue;
            }

            equipment.AddHeroEquipment(data.Id);
            count++;
        }

        Logger.Log($"[EquipmentDebug] 장비 {count}개 지급");
    }

    private void RemoveAll(HeroEquipmentModel equipment, HeroEquipedModel equiped)
    {
        UnEquipAll(equiped);

        List<string> ownedIds = new List<string>(equipment.Equipments.Keys);

        for (int i = 0; i < ownedIds.Count; i++)
        {
            equipment.RemoveHeroEquipment(ownedIds[i]);
        }

        Logger.Log($"[EquipmentDebug] 장비 {ownedIds.Count}개 제거");
    }

    private void UnEquipAll(HeroEquipedModel equiped)
    {
        for (int i = 0; i < EQUIPMENT_TYPES.Length; i++)
        {
            if (string.IsNullOrEmpty(equiped.GetEquipedId(EQUIPMENT_TYPES[i])))
            {
                continue;
            }

            equiped.UnEquip(EQUIPMENT_TYPES[i]);
        }
    }

    private void DrawFilter()
    {
        EditorGUILayout.LabelField("필터", EditorStyles.boldLabel);

        _typeFilterIndex = EditorGUILayout.Popup("타입", _typeFilterIndex, TYPE_FILTER_LABELS);
        _gradeFilterIndex = EditorGUILayout.Popup("등급", _gradeFilterIndex, GRADE_FILTER_LABELS);
        _searchText = EditorGUILayout.TextField("이름 / ID", _searchText);
        _showOwnedOnly = EditorGUILayout.Toggle("보유한 것만", _showOwnedOnly);
    }

    private void DrawEquipmentList(HeroEquipmentModel equipment, HeroEquipedModel equiped)
    {
        int visibleCount = 0;

        for (int i = 0; i < _sortedEquipments.Count; i++)
        {
            EquipmentData data = _sortedEquipments[i];

            if (!IsVisible(data, equipment))
            {
                continue;
            }

            DrawEquipmentRow(data, equipment, equiped);
            visibleCount++;
        }

        if (0 == visibleCount)
        {
            EditorGUILayout.HelpBox("조건에 맞는 장비가 없습니다.", MessageType.Info);
        }
    }

    private bool IsVisible(EquipmentData data, HeroEquipmentModel equipment)
    {
        if (0 != _typeFilterIndex)
        {
            EquipmentType equipmentType = Utils.ParseEnum<EquipmentType>(data.EquipmentTypeString);

            if (equipmentType != EQUIPMENT_TYPES[_typeFilterIndex - 1])
            {
                return false;
            }
        }

        if (0 != _gradeFilterIndex && data.EquipmentGrade != EQUIPMENT_GRADES[_gradeFilterIndex - 1])
        {
            return false;
        }

        if (_showOwnedOnly && null == equipment.GetHeroEquipment(data.Id))
        {
            return false;
        }

        return IsSearchMatched(data);
    }

    private bool IsSearchMatched(EquipmentData data)
    {
        if (string.IsNullOrEmpty(_searchText))
        {
            return true;
        }

        if (null != data.Name && data.Name.IndexOf(_searchText, System.StringComparison.OrdinalIgnoreCase) >= 0)
        {
            return true;
        }

        return data.Id.IndexOf(_searchText, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private void DrawEquipmentRow(EquipmentData data, HeroEquipmentModel equipment, HeroEquipedModel equiped)
    {
        bool isOwned = null != equipment.GetHeroEquipment(data.Id);
        EquipmentType equipmentType = Utils.ParseEnum<EquipmentType>(data.EquipmentTypeString);
        bool isEquiped = data.Id == equiped.GetEquipedId(equipmentType);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(BuildRowLabel(data, equipmentType, isEquiped), GUILayout.MinWidth(180f));
        EditorGUILayout.LabelField(BuildStateLabel(isOwned, equipment.GetLevel(data.Id)), GUILayout.Width(STATE_WIDTH));

        DrawGiveButton(data, equipment, isOwned);
        DrawRemoveButton(data, equipment, equiped, isOwned, isEquiped);
        DrawLevelUpButton(data, equipment, isOwned);
        DrawEquipButton(data, equiped, equipmentType, isOwned, isEquiped);

        EditorGUILayout.EndHorizontal();
    }

    private string BuildRowLabel(EquipmentData data, EquipmentType equipmentType, bool isEquiped)
    {
        string displayName = string.IsNullOrEmpty(data.Name) ? data.Id : data.Name;
        string equipedMark = isEquiped ? " [착용]" : string.Empty;

        return $"{displayName} ({data.EquipmentGrade} / {GetTypeName(equipmentType)}){equipedMark}";
    }

    private string BuildStateLabel(bool isOwned, int level)
    {
        return isOwned ? $"Lv.{level}" : "미보유";
    }

    private void DrawGiveButton(EquipmentData data, HeroEquipmentModel equipment, bool isOwned)
    {
        EditorGUI.BeginDisabledGroup(isOwned);

        if (GUILayout.Button("지급", GUILayout.Width(BUTTON_WIDTH)))
        {
            equipment.AddHeroEquipment(data.Id);
            Logger.Log($"[EquipmentDebug] {data.Name} 지급");
        }

        EditorGUI.EndDisabledGroup();
    }

    private void DrawRemoveButton(EquipmentData data, HeroEquipmentModel equipment, HeroEquipedModel equiped, bool isOwned, bool isEquiped)
    {
        EditorGUI.BeginDisabledGroup(!isOwned);

        if (GUILayout.Button("제거", GUILayout.Width(BUTTON_WIDTH)))
        {
            // 착용 중인 장비를 그냥 지우면 착용 슬롯에 없는 장비 id가 남는다.
            if (isEquiped)
            {
                equiped.UnEquip(Utils.ParseEnum<EquipmentType>(data.EquipmentTypeString));
            }

            equipment.RemoveHeroEquipment(data.Id);
            Logger.Log($"[EquipmentDebug] {data.Name} 제거");
        }

        EditorGUI.EndDisabledGroup();
    }

    private void DrawLevelUpButton(EquipmentData data, HeroEquipmentModel equipment, bool isOwned)
    {
        EditorGUI.BeginDisabledGroup(!isOwned);

        if (GUILayout.Button("+Lv", GUILayout.Width(BUTTON_WIDTH)))
        {
            LevelUpResult result = equipment.TryLevelUp(data.Id);

            Logger.Log($"[EquipmentDebug] {data.Name} 강화 : {result}");
        }

        EditorGUI.EndDisabledGroup();
    }

    private void DrawEquipButton(EquipmentData data, HeroEquipedModel equiped, EquipmentType equipmentType, bool isOwned, bool isEquiped)
    {
        EditorGUI.BeginDisabledGroup(!isOwned || isEquiped);

        if (GUILayout.Button("장착", GUILayout.Width(BUTTON_WIDTH)))
        {
            equiped.Equip(equipmentType, data.Id);
            Logger.Log($"[EquipmentDebug] {data.Name} 장착");
        }

        EditorGUI.EndDisabledGroup();
    }

    private string GetTypeName(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                return "무기";

            case EquipmentType.Armor:
                return "방어구";

            case EquipmentType.Accessory:
                return "장신구";
        }

        return equipmentType.ToString();
    }
}
