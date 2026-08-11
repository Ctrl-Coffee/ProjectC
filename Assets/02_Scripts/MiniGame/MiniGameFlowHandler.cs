using Cysharp.Threading.Tasks;
using System;
using System.Threading;

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

    public async UniTaskVoid StartMiniGameAsync()
    {
        MiniGameResult result = await PlaySubtitleEditAsync();

        if (!result.IsCompleted)
        {
            return;
        }

        string message = $"{result.Grade} / 정확도 {result.Accuracy:P0}";
        Logger.Log($"미니게임 종료 - {message}");
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
