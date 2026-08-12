using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class MiniGameFlowHandler
{
    private int _workLevel = 1;
    private CancellationTokenSource _cancelToken = new();

    public void Cancel()
    {
        if (_cancelToken.IsCancellationRequested)
        {
            return;
        }

        _cancelToken.Cancel();
    }

    public async UniTaskVoid StartMiniGameAsync(WorkData workData)
    {
        if (null == workData)
        {
            Logger.LogError("업무 데이터가 없어 미니게임을 시작할 수 없습니다.");
            return;
        }

        MiniGameResult result = await PlayAsync(workData.MiniGameType);

        if (!result.IsCompleted)
        {
            return;
        }

        GiveReward(workData, result.Accuracy);

        Logger.Log($"미니게임 종료 - {result.Grade} / 정확도 {result.Accuracy:P0}");
    }

    private void GiveReward(WorkData workData, float accuracy)
    {
        float rate = Mathf.Clamp01(accuracy);

        long money = Mathf.RoundToInt(workData.RewardMoney * rate);
        long dp = Mathf.RoundToInt(workData.RewardDP * rate);

        if (money <= 0 && dp <= 0)
        {
            Logger.Log($"수동업무 보상 없음 - {workData.Name}");
            return;
        }

        GameManager.User.Currency.AddMoney(money);
        GameManager.User.Currency.AddDreamPoint(dp);

        GameManager.Save.Save();

        Logger.Log($"수동업무 완료 - {workData.Name} / 돈 {money} / DP {dp}");
    }

    private UniTask<MiniGameResult> PlayAsync(MiniGameType miniGameType)
    {
        switch (miniGameType)
        {
            case MiniGameType.SubtitleEdit:
                return PlaySubtitleEditAsync();

            default:
                Logger.LogError($"지원하지 않는 미니게임입니다. type: {miniGameType}");
                return UniTask.FromResult(MiniGameResult.Canceled);
        }
    }

    private async UniTask<MiniGameResult> PlaySubtitleEditAsync()
    {
        SubtitleEditGameUI ui = await GameManager.UI.OpenSubtitleEditGameUI();

        if (null == ui)
        {
            return MiniGameResult.Canceled;
        }

        MiniGameContext context = new MiniGameContext
        {
            WorkLevel = _workLevel,
        };

        CancellationToken token = _cancelToken.Token;

        try
        {
            MiniGameResult result = await ui.RunAsync(context, token);

            if (result.IsCompleted)
            {
                await ShowResultAsync(result, token);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return MiniGameResult.Canceled;
        }
        finally
        {
            ui.CloseUI();
        }
    }

    private async UniTask ShowResultAsync(MiniGameResult result, CancellationToken token)
    {
        MiniGameResultUI resultUI = await GameManager.UI.OpenMiniGameResultUI();

        if (null == resultUI)
        {
            Logger.LogWarning("결과창을 열 수 없어 건너뜁니다.");
            return;
        }

        resultUI.SetResult(result);
        await resultUI.WaitForCloseAsync(token);
    }
}
