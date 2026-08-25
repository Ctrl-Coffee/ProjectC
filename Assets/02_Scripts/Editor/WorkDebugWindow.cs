using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WorkDebugWindow : EditorWindow
{
    private const long ADD_INSPIRATION_AMOUNT = 10;
    private const double REPAINT_INTERVAL = 0.25;

    private static readonly string[] TAB_NAMES = { "기본", "수동 업무", "자동 업무", "에너지" };

    private int _tabIndex = 0;
    private Vector2 _scrollPosition;
    private double _nextRepaintTime;

    private Dictionary<WorkStatType, GUIContent> _statLabels = new();
    private GUIContent _valueContent = new GUIContent();

    [MenuItem("Tools/Work Debug")]
    private static void Open()
    {
        GetWindow<WorkDebugWindow>("Work Debug");
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            _statLabels.Clear();
            return;
        }

        if (EditorApplication.timeSinceStartup < _nextRepaintTime)
        {
            return;
        }

        _nextRepaintTime = EditorApplication.timeSinceStartup + REPAINT_INTERVAL;

        Repaint();
    }

    private void OnGUI()
    {
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("플레이 모드에서만 동작합니다.", MessageType.Info);
            return;
        }

        if (null == GameManager.Instance)
        {
            EditorGUILayout.HelpBox("GameManager가 아직 준비되지 않았습니다.", MessageType.Warning);
            return;
        }

        _tabIndex = GUILayout.Toolbar(_tabIndex, TAB_NAMES);
        EditorGUILayout.Space();

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        switch (_tabIndex)
        {
            case 1:
                DrawManualWorkTab();
                break;

            case 2:
                DrawAutoWorkTab();
                break;

            case 3:
                DrawEnergyTab();
                break;

            default:
                DrawBasicTab();
                break;
        }

        EditorGUILayout.EndScrollView();
    }

    #region 기본 탭

    private void DrawBasicTab()
    {
        DrawTime();
        EditorGUILayout.Space();

        DrawQueue();
        EditorGUILayout.Space();

        DrawCurrency();
        EditorGUILayout.Space();

        DrawUnlockedPerks();
    }

    private void DrawTime()
    {
        EditorGUILayout.LabelField("시간", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("디버그 오프셋", GameManager.Time.DebugTimeOffset.ToString());

        EditorGUILayout.BeginHorizontal();

        DrawAddTimeButton("+1분", TimeSpan.FromMinutes(1));
        DrawAddTimeButton("+10분", TimeSpan.FromMinutes(10));
        DrawAddTimeButton("+1시간", TimeSpan.FromHours(1));

        if (GUILayout.Button("초기화"))
        {
            ResetDebugTime();
        }

        EditorGUILayout.EndHorizontal();
    }

    private void ResetDebugTime()
    {
        long shiftTicks = -GameManager.Time.DebugTimeOffset.Ticks;

        GameManager.Time.ResetDebugTime();

        if (0 == shiftTicks)
        {
            return;
        }

        AutoWorkQueue.DebugShiftSchedule(shiftTicks);

        EnergyRecovery.DebugShiftLastRecoverTicks(shiftTicks);
    }

    private void DrawAddTimeButton(string label, TimeSpan amount)
    {
        if (!GUILayout.Button(label))
        {
            return;
        }

        GameManager.Time.AddDebugTime(amount);
    }

    private void DrawQueue()
    {
        EditorGUILayout.LabelField("자동업무 큐", EditorStyles.boldLabel);

        EditorGUILayout.LabelField("등록", $"{AutoWorkQueue.Count} / {AutoWorkQueue.MaxSlotCount}");
        EditorGUILayout.LabelField("총 남은시간", Utils.FormatClock(AutoWorkQueue.GetTotalRemainSeconds()));

        for (int i = 0; i < AutoWorkQueue.Count; i++)
        {
            EditorGUILayout.LabelField($"    {i}번", $"{AutoWorkQueue.GetProgress(i) * 100f:0.0}%");
        }

        EditorGUI.BeginDisabledGroup(AutoWorkQueue.Count == 0);

        if (GUILayout.Button("첫 작업 완료"))
        {
            AutoWorkQueue.DebugCompleteFirst();
        }

        EditorGUI.EndDisabledGroup();
    }

    private void DrawCurrency()
    {
        EditorGUILayout.LabelField("재화", EditorStyles.boldLabel);

        CurrencyModel currency = GameManager.Session.Currency;

        EditorGUILayout.LabelField("돈", currency.Money.ToString());
        EditorGUILayout.LabelField("드림 포인트", currency.DreamPoint.ToString());
        EditorGUILayout.LabelField("에너지", $"{currency.Energy} / {currency.MaxEnergy}");
        EditorGUILayout.LabelField("꿈의 조각", currency.DreamFragment.ToString());
        EditorGUILayout.LabelField("몽상의 스크롤", currency.DreamScroll.ToString());
        EditorGUILayout.LabelField("영감", currency.Inspiration.ToString());

        if (GUILayout.Button("영감 +10"))
        {
            currency.AddInspiration(ADD_INSPIRATION_AMOUNT);
        }
    }

    private void DrawUnlockedPerks()
    {
        IReadOnlyList<string> perkIds = GameManager.Perk.GetUnlockedPerkIds();

        EditorGUILayout.LabelField($"활성 퍽 ({perkIds.Count}개)", EditorStyles.boldLabel);

        if (perkIds.Count == 0)
        {
            EditorGUILayout.LabelField("    없음");
            return;
        }

        for (int i = 0; i < perkIds.Count; i++)
        {
            PerkNodeData data = GameManager.DataTable.GetPerkNodeData(perkIds[i]);
            string name = null != data ? data.Name : "(테이블 없음)";

            EditorGUILayout.LabelField($"    {perkIds[i]}", name);
        }
    }

    #endregion

    #region 업무 버프 탭

    private void DrawManualWorkTab()
    {
        EditorGUILayout.LabelField("수동 업무 보정", EditorStyles.boldLabel);

        DrawModifier(WorkStatType.ManualWorkRewardMoney);
        DrawModifier(WorkStatType.ManualWorkRewardDP);
        DrawModifier(WorkStatType.WorkEnergyCost);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("업무별 적용 결과", EditorStyles.boldLabel);

        DrawWorkList(WorkType.Manual);
    }

    private void DrawAutoWorkTab()
    {
        EditorGUILayout.LabelField("자동 업무 보정", EditorStyles.boldLabel);

        DrawModifier(WorkStatType.AutoWorkRewardMoney);
        DrawModifier(WorkStatType.AutoWorkRewardDP);
        DrawModifier(WorkStatType.WorkDuration);
        DrawModifier(WorkStatType.AutoWorkSlotCount);

        EditorGUILayout.Space();

        DrawCompare("적용 슬롯 수", AutoWorkQueue.BaseSlotCount, AutoWorkQueue.MaxSlotCount, "0");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("업무별 적용 결과", EditorStyles.boldLabel);

        DrawWorkList(WorkType.Auto);
    }

    private void DrawWorkList(WorkType workType)
    {
        PerkStatCalculator stat = GameManager.Perk.Stat;

        bool isAuto = WorkType.Auto == workType;

        WorkStatType moneyStat = isAuto ? WorkStatType.AutoWorkRewardMoney : WorkStatType.ManualWorkRewardMoney;
        WorkStatType dpStat = isAuto ? WorkStatType.AutoWorkRewardDP : WorkStatType.ManualWorkRewardDP;

        foreach (WorkData data in GameManager.DataTable.WorkDataTable.Values)
        {
            if (workType != data.Type)
            {
                continue;
            }

            bool isUnlocked = GameManager.Perk.Unlock.IsUnlocked(data.Id);
            string lockMark = isUnlocked ? string.Empty : "  [잠김]";

            EditorGUILayout.LabelField($"{data.Name}{lockMark}", data.Id, EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            DrawCompare("보상 돈", data.RewardMoney, stat.GetLong(moneyStat, data.RewardMoney), "0");
            DrawCompare("보상 DP", data.RewardDP, stat.GetLong(dpStat, data.RewardDP), "0");

            if (isAuto)
            {
                DrawCompare("소요 시간(초)", data.DurationSeconds, stat.GetFloat(WorkStatType.WorkDuration, data.DurationSeconds), "0");
            }
            else
            {
                DrawCompare("에너지 소모", data.ReqEnergy, stat.GetLong(WorkStatType.WorkEnergyCost, data.ReqEnergy), "0");
            }

            EditorGUI.indentLevel--;
        }
    }

    #endregion

    #region 에너지 탭

    private void DrawEnergyTab()
    {
        CurrencyModel currency = GameManager.Session.Currency;

        EditorGUILayout.LabelField("최대 에너지", EditorStyles.boldLabel);

        DrawModifier(WorkStatType.EnergyMax);
        DrawCompare("최대치", currency.BaseMaxEnergy, currency.MaxEnergy, "0");
        EditorGUILayout.LabelField("현재", $"{currency.Energy} / {currency.MaxEnergy}");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("회복 속도", EditorStyles.boldLabel);

        DrawModifier(WorkStatType.EnergyRecoverRate);

        float baseInterval = EnergyRecovery.BaseIntervalSeconds;
        float appliedInterval = EnergyRecovery.RecoverIntervalSeconds;

        float speed = EnergyRecovery.RecoverSpeed;

        EditorGUILayout.LabelField("속도", $"x{speed:0.##}   ({FormatPercent(speed - 1f)})");
        DrawCompare("회복 주기(초)", baseInterval, appliedInterval, "0.##");

        if (0f < appliedInterval && 0f < baseInterval)
        {
            DrawCompare("분당 회복량", 60f / baseInterval, 60f / appliedInterval, "0.##");
        }
    }

    #endregion

    #region 공통

    private void DrawModifier(WorkStatType statType)
    {
        GUIContent label = GetStatLabel(statType);

        if (!GameManager.Perk.Stat.TryGetModifier(statType, out float flat, out float additiveRate, out float compoundRate))
        {
            _valueContent.text = "보정 없음";
            EditorGUILayout.LabelField(label, _valueContent);
            return;
        }

        float totalRate = (1f + additiveRate) * compoundRate;

        string detail = $"합 {FormatPercent(additiveRate)} / 곱 {FormatPercent(compoundRate - 1f)}";

        if (0f != flat)
        {
            detail += $" / 고정 {flat:+0.##;-0.##;0}";
        }

        _valueContent.text = $"{FormatPercent(totalRate - 1f)}   ({detail})";

        EditorGUILayout.LabelField(label, _valueContent);
    }

    private void DrawCompare(string label, double baseValue, double appliedValue, string format)
    {
        string diff = string.Empty;

        if (0d != baseValue)
        {
            diff = $"   ({FormatPercent(appliedValue / baseValue - 1d)})";
        }

        EditorGUILayout.LabelField(label, $"{baseValue.ToString(format)} -> {appliedValue.ToString(format)}{diff}");
    }

    private GUIContent GetStatLabel(WorkStatType statType)
    {
        if (_statLabels.TryGetValue(statType, out GUIContent cached))
        {
            return cached;
        }

        GUIContent label = CreateStatLabel(statType);

        _statLabels.Add(statType, label);

        return label;
    }

    private GUIContent CreateStatLabel(WorkStatType statType)
    {
        WorkStatData data = GameManager.DataTable.GetWorkStatData(statType);

        if (null == data)
        {
            return new GUIContent(statType.ToString(), "WorkStat 테이블에 정의가 없습니다.");
        }

        string name = string.IsNullOrEmpty(data.Name) ? statType.ToString() : data.Name;

        return new GUIContent(name, data.Description);
    }

    private static string FormatPercent(double rate)
    {
        return $"{rate * 100d:+0.#;-0.#;0}%";
    }

    #endregion
}
