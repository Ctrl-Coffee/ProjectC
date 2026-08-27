using System.Diagnostics.Metrics;
using UnityEngine;

public class TestNovel : MonoBehaviour
{
    private NovelCatchLogic _logic = new();

    [ContextMenu("1. 텍스트 생성 확인")]
    private void TestCreateKeyText()
    {
        for (int i = 0; i < 10; i++)
        {
            Logger.Log(_logic.CreateKeyText());
        }
    }

    [ContextMenu("2. 영역 생성 범위 검사")]
    private void TestCreateZone()
    {
        int failCount = 0;

        for (int i = 0; i < 10000; i++)
        {
            CatchZone zone = _logic.CreateZone(0.2f);

            if (zone.Left < 0f || zone.Right > 1f)
            {
                failCount++;
            }
        }

        if (failCount == 0)
        {
            Logger.Log("검사 통과");
        }
        else
        {
            Logger.LogError($"{failCount} 회 실패");
        }
    }

    [ContextMenu("3. 키 중심 범위 검사")]
    private void TestGetKeyCenter()
    {
        int failCount = 0;

        for (int i = 0; i < 10000; i++)
        {
            float t = i * 0.01f;
            float center = _logic.GetKeyCenter(t, 0.2f);

            if (center < 0.0999f || center > 0.9001f)
            {
                failCount++;
            }
        }

        if (failCount == 0)
        {
            Logger.Log("키 중심 검사 통과");
        }
        else
        {
            Logger.LogError($"{failCount} 회 실패");
        }
    }

    [ContextMenu("4. 판정 경계 검사")]
    private void TestJudge()
    {
        CatchZone zone = new CatchZone { Left = 0.3f, Right = 0.7f };

        CheckJudge("딱 맞음", 0.3f, 0.7f, zone, true);
        CheckJudge("완전 안쪽", 0.4f, 0.6f, zone, true);
        CheckJudge("왼쪽 걸침", 0.1f, 0.5f, zone, false);
        CheckJudge("완전 밖", 0.8f, 1.0f, zone, false);
    }

    private void CheckJudge(string caseName, float keyLeft, float keyRight, CatchZone zone, bool expected)
    {
        bool result = _logic.Judge(keyLeft, keyRight, zone);

        if (result == expected)
        {
            Logger.Log("검사 통과");
        }
        else
        {
            Logger.LogError($"{caseName} 실패 — 기대 {expected}, 실제 {result}");
        }
    }
}
