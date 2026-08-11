using System;
using UnityEditor;
using UnityEngine;

public class WorkDebugWindow : EditorWindow
{
    [MenuItem("Tools/Work Debug")]
    private static void Open()
    {
        GetWindow<WorkDebugWindow>("Work Debug");
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            return;
        }

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

        DrawTime();
        EditorGUILayout.Space();

        DrawQueue();
        EditorGUILayout.Space();

        DrawCurrency();
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
            GameManager.Time.ResetDebugTime();
        }

        EditorGUILayout.EndHorizontal();
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

        EditorGUILayout.LabelField("등록", $"{AutoWorkQueue.Count} / {AutoWorkQueue.MAX_SLOT_COUNT}");
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

        CurrencyModel currency = GameManager.User.Currency;

        EditorGUILayout.LabelField("돈", currency.Money.ToString());
        EditorGUILayout.LabelField("DP", currency.DP.ToString());
    }
}
