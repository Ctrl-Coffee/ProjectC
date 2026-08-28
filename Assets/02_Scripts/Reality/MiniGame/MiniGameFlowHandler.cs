using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class MiniGameFlowHandler
{
    private CancellationTokenSource _cancelToken;

    private bool _isPlaying = false;

    public void Cancel()
    {
        if (null == _cancelToken || _cancelToken.IsCancellationRequested)
        {
            return;
        }

        _cancelToken.Cancel();
    }

    public async UniTask StartMiniGameAsync(WorkData workData)
    {
        if (null == workData)
        {
            Logger.LogError("업무 데이터가 없어 미니게임을 시작할 수 없습니다.");
            return;
        }

        if (_isPlaying)
        {
            return;
        }

        long energyCost = GetEnergyCost(workData);
        long goldCost = GetGoldCost(workData);

        if (!GameManager.Session.Currency.CanSpendEnergy(energyCost))
        {
            Logger.LogWarning($"에너지가 부족해 시작할 수 없습니다. {workData.Name} / 필요 {energyCost} / 보유 {GameManager.Session.Currency.Energy}");
            return;
        }

        if (0 < goldCost && !GameManager.Session.Currency.CanSpendMoney(goldCost))
        {
            Logger.LogWarning($"골드가 부족해 시작할 수 없습니다. {workData.Name} / 필요 {goldCost} / 보유 {GameManager.Session.Currency.Money}");
            return;
        }

        _isPlaying = true;

        try
        {
            MiniGameResult result = await PlayAsync(workData);

            if (!result.IsCompleted)
            {
                return;
            }

            GiveReward(workData, result);

            Logger.Log($"미니게임 종료 - {result.Grade} / 정확도 {result.Accuracy:P0}");
        }
        finally
        {
            _isPlaying = false;
        }
    }

    private long GetEnergyCost(WorkData workData)
    {
        return GameManager.Perk.Stat.GetLong(WorkStatType.WorkEnergyCost, workData.ReqEnergy);
    }

    private long GetGoldCost(WorkData workData)
    {
        // Perk 추가시 대체 return GameManager.Perk.Stat.GetLong(WorkStatType.WorkGoldCost, workData.ReqGold);
        return workData.ReqGold;
    }

    private void RefundCost(long energyCost, long goldCost)
    {
        if (0 < energyCost)
        {
            GameManager.Session.Currency.AddEnergy(energyCost);
        }

        if (0 < goldCost)
        {
            GameManager.Session.Currency.AddMoney(goldCost);
        }

        Logger.Log($"미니게임 비용 반환 - 에너지 {energyCost} / 골드 {goldCost}");
    }
    private void GiveReward(WorkData workData, MiniGameResult result)
    {
        float rate = Mathf.Clamp01(result.Accuracy) * result.RewardMultiplier;

        long money = (long)Math.Round(GameManager.Perk.Stat.GetFloat(WorkStatType.ManualWorkRewardMoney, workData.RewardMoney) * (double)rate);
        long dp = (long)Math.Round(GameManager.Perk.Stat.GetFloat(WorkStatType.ManualWorkRewardDP, workData.RewardDP) * (double)rate);

        if (money <= 0 && dp <= 0)
        {
            Logger.Log($"수동업무 보상 없음 - {workData.Name}");
            return;
        }

        GameManager.Session.Currency.AddMoney(money);
        GameManager.Session.Currency.AddDreamPoint(dp);

        Logger.Log($"수동업무 완료 - {workData.Name} / 돈 {money} / DP {dp}");
    }

    private UniTask<MiniGameResult> PlayAsync(WorkData workData)
    {
        switch (workData.MiniGameType)
        {
            case MiniGameType.SubtitleEdit:
                return PlayMiniGameAsync<SubtitleEditGameUI>(workData);

            case MiniGameType.MotionTracking:
                return PlayMiniGameAsync<MotionTrackingGameUI>(workData);

            case MiniGameType.DiceGamble:
                return PlayMiniGameAsync<DiceGambleGameUI>(workData);

            case MiniGameType.ScratchLottery:
                return PlayMiniGameAsync<ScratchLotteryGameUI>(workData);

            case MiniGameType.NovelWriting:
                return PlayMiniGameAsync<NovelWritingGameUI>(workData);

            default:
                Logger.LogError($"지원하지 않는 미니게임입니다. type: {workData.MiniGameType}");
                return UniTask.FromResult(MiniGameResult.Canceled);
        }
    }

    private async UniTask<MiniGameResult> PlayMiniGameAsync<T>(WorkData workData) where T : MiniGameBase
    {
        T ui = await GameManager.UI.OpenMiniGameUI<T>();

        if (null == ui)
        {
            Logger.LogWarning($"미니게임 UI를 열지 못했습니다. type: {workData.MiniGameType}");
            return MiniGameResult.Canceled;
        }

        long energyCost = GetEnergyCost(workData);
        long goldCost = GetGoldCost(workData);

        bool isRoundCostType = workData.MiniGameType == MiniGameType.NovelWriting;

        if (isRoundCostType == false)
        {
            if (!GameManager.Session.Currency.TrySpendEnergy(energyCost))
            {
                Logger.LogError($"에너지 차감에 실패했습니다. {workData.Name} / 필요 {energyCost}");
                ui.CloseUI();
                return MiniGameResult.Canceled;
            }

            if (0 < goldCost && !GameManager.Session.Currency.TrySpendMoney(goldCost))
            {
                Logger.LogError($"골드 차감에 실패했습니다. {workData.Name} / 필요 {goldCost}");

                RefundCost(energyCost, 0);

                ui.CloseUI();
                return MiniGameResult.Canceled;
            }
        }

        Logger.Log($"미니게임 시작 - {workData.Name} / 남은 에너지 {GameManager.Session.Currency.Energy} / 남은 골드 {GameManager.Session.Currency.Money}");

        MiniGameContext context = new MiniGameContext { EnergyCost = energyCost };

        _cancelToken = new CancellationTokenSource();
        CancellationToken token = _cancelToken.Token;

        MiniGameResult result = MiniGameResult.Canceled;

        try
        {
            result = await ui.RunAsync(context, token);

            if (result.IsCompleted && result.SkipResultPopup == false)
            {
                await ShowResultAsync(result, token);
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            return result;
        }
        finally
        {
            if (result.IsCompleted == false)
            {
                RefundCost(isRoundCostType ? 0 : energyCost, goldCost);
            }

            ui.CloseUI();

            _cancelToken.Dispose();
            _cancelToken = null;
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
