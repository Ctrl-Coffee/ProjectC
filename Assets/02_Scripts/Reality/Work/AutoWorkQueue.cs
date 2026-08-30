using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public static class AutoWorkQueue
{
    private const int BASE_SLOT_COUNT = 5;
    private const float COLLECT_INTERVAL = 1f;

    public static event Action OnQueueChanged;

    private static List<AutoWorkSlot> _slots = new();

    public static int MaxSlotCount
    {
        get
        {
            return GameManager.Perk.Stat.GetInt(WorkStatType.AutoWorkSlotCount, BASE_SLOT_COUNT);
        }
    }

    private static List<AutoWorkSlot> Slots
    {
        get
        {
            return _slots;
        }
    }

    public static int Count
    {
        get
        {
            return Slots.Count;
        }
    }

    public static bool CanEnqueue
    {
        get
        {
            return Slots.Count < MaxSlotCount;
        }
    }

    public static bool TryEnqueue(string workId)
    {
        if (!CanEnqueue)
        {
            Logger.LogWarning($"업무 큐가 가득 찼습니다. (최대 {MaxSlotCount}개)");
            return false;
        }

        WorkData data = GameManager.DataTable.GetWorkData(workId);

        if (null == data)
        {
            Logger.LogError($"업무 데이터를 찾을 수 없습니다. id: {workId}");
            return false;
        }

        if (WorkType.Auto != data.Type)
        {
            Logger.LogError($"자동업무가 아니어서 큐에 넣을 수 없습니다. id: {workId}");
            return false;
        }

        if (!GameManager.Perk.Unlock.IsUnlocked(workId))
        {
            Logger.LogWarning($"아직 해금되지 않은 업무입니다. id: {workId}");
            return false;
        }

        List<AutoWorkSlot> slots = Slots;
        long startTicks = GameManager.Time.UtcNow.Ticks;

        if (slots.Count > 0)
        {
            startTicks = Math.Max(startTicks, slots[slots.Count - 1].EndTicks);
        }

        slots.Add(new AutoWorkSlot
        {
            WorkId = workId,
            StartTicks = startTicks,
            EndTicks = startTicks + GetDurationTicks(data),
        });

        NotifyQueueChanged();

        return true;
    }

    private static void NotifyQueueChanged()
    {
        OnQueueChanged?.Invoke();
    }

    public static void NormalizeSchedule()
    {
        List<AutoWorkSlot> slots = Slots;

        if (slots.Count == 0)
        {
            return;
        }

        long nowTicks = GameManager.Time.UtcNow.Ticks;
        long shiftTicks = slots[0].StartTicks - nowTicks;

        if (shiftTicks <= 0)
        {
            return;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            AutoWorkSlot slot = slots[i];

            slot.StartTicks -= shiftTicks;
            slot.EndTicks -= shiftTicks;

            slots[i] = slot;
        }

        NotifyQueueChanged();

        Logger.LogWarning($"미래 시각의 자동업무 큐를 {shiftTicks / TimeSpan.TicksPerSecond}초 앞당겼습니다.");
    }

    public static bool TryCancel(int index)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        Slots.RemoveAt(index);
        RecalculateFrom(index);

        NotifyQueueChanged();

        return true;
    }

    private static void RecalculateFrom(int startIndex)
    {
        List<AutoWorkSlot> slots = Slots;
        long nowTicks = GameManager.Time.UtcNow.Ticks;

        for (int i = startIndex; i < slots.Count; i++)
        {
            WorkData data = GameManager.DataTable.GetWorkData(slots[i].WorkId);

            if (null == data)
            {
                Logger.LogError($"업무 데이터를 찾을 수 없어 시각 재계산을 건너뜁니다. id: {slots[i].WorkId}");
                continue;
            }

            long startTicks = nowTicks;

            if (i > 0)
            {
                startTicks = Math.Max(nowTicks, slots[i - 1].EndTicks);
            }

            AutoWorkSlot slot = slots[i];

            slot.StartTicks = startTicks;
            slot.EndTicks = startTicks + GetDurationTicks(data);

            slots[i] = slot;
        }
    }

    private static long GetDurationTicks(WorkData data)
    {
        float durationSeconds = GameManager.Perk.Stat.GetFloat(WorkStatType.WorkDuration, data.DurationSeconds);
        return (long)(durationSeconds * TimeSpan.TicksPerSecond);
    }

    public static async UniTaskVoid RunCollectLoopAsync(CancellationToken token)
    {
        NormalizeSchedule();

        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(COLLECT_INTERVAL), ignoreTimeScale: true, cancellationToken: token);

            // [주의] 자리비움 리포트가 뜨는 동안 정산을 미룬다. 리포트를 닫으면 풀린다.
            // 자동업무 보상이 안 들어오면 여기서 계속 걸러지고 있는지부터 확인할 것.
            if (AwayRewardPayout.IsHolding)
            {
                continue;
            }

            CollectCompleted();
        }
    }

    public static int CollectCompleted()
    {
        Reward reward = ConsumeCompleted();

        Pay(reward);

        return reward.Count;
    }

    public static Reward ConsumeCompleted()
    {
        List<AutoWorkSlot> slots = Slots;
        long nowTicks = GameManager.Time.UtcNow.Ticks;

        Reward reward = new Reward();

        while (slots.Count > 0 && nowTicks >= slots[0].EndTicks)
        {
            WorkData data = GameManager.DataTable.GetWorkData(slots[0].WorkId);

            slots.RemoveAt(0);
            reward.Count++;

            if (null == data)
            {
                Logger.LogError("정산할 업무 데이터를 찾을 수 없어 보상을 건너뜁니다.");
                continue;
            }

            long money = GameManager.Perk.Stat.GetLong(WorkStatType.AutoWorkRewardMoney, data.RewardMoney);
            long dp = GameManager.Perk.Stat.GetLong(WorkStatType.AutoWorkRewardDP, data.RewardDP);

            reward.Money += money;
            reward.DreamPoint += dp;

            reward.AddWorkCount(data.Id);

            Logger.Log($"자동업무 완료 - {data.Name} / 돈 {money} / DP {dp}");
        }

        if (reward.Count > 0)
        {
            NotifyQueueChanged();
        }

        return reward;
    }

    public static Reward PeekCompletedReward()
    {
        List<AutoWorkSlot> slots = Slots;
        long nowTicks = GameManager.Time.UtcNow.Ticks;

        Reward reward = new Reward();

        for (int i = 0; i < slots.Count; i++)
        {
            if (nowTicks < slots[i].EndTicks)
            {
                break;
            }

            WorkData data = GameManager.DataTable.GetWorkData(slots[i].WorkId);

            reward.Count++;

            if (null == data)
            {
                continue;
            }

            reward.Money += GameManager.Perk.Stat.GetLong(WorkStatType.AutoWorkRewardMoney, data.RewardMoney);
            reward.DreamPoint += GameManager.Perk.Stat.GetLong(WorkStatType.AutoWorkRewardDP, data.RewardDP);

            reward.AddWorkCount(data.Id);
        }

        return reward;
    }

    private static void Pay(Reward reward)
    {
        if (0 < reward.Money)
        {
            GameManager.Session.Currency.AddMoney(reward.Money);
        }

        if (0 < reward.DreamPoint)
        {
            GameManager.Session.Currency.AddDreamPoint(reward.DreamPoint);
        }
    }

    public struct Reward
    {
        public long Money;
        public long DreamPoint;
        public int Count;

        public Dictionary<string, int> WorkCounts;

        public void AddWorkCount(string workId)
        {
            if (string.IsNullOrEmpty(workId))
            {
                return;
            }

            if (null == WorkCounts)
            {
                WorkCounts = new Dictionary<string, int>();
            }

            WorkCounts.TryGetValue(workId, out int count);
            WorkCounts[workId] = count + 1;
        }
    }

    public static float GetTotalRemainSeconds()
    {
        List<AutoWorkSlot> slots = Slots;

        if (slots.Count == 0)
        {
            return 0f;
        }

        long remainTicks = slots[slots.Count - 1].EndTicks - GameManager.Time.UtcNow.Ticks;

        if (remainTicks <= 0)
        {
            return 0f;
        }

        return (float)remainTicks / TimeSpan.TicksPerSecond;
    }

    public static string GetWorkId(int index)
    {
        if (!IsValidIndex(index))
        {
            return string.Empty;
        }

        return Slots[index].WorkId;
    }

    public static float GetProgress(int index)
    {
        if (!IsValidIndex(index))
        {
            return 0f;
        }

        AutoWorkSlot slot = Slots[index];
        long totalTicks = slot.EndTicks - slot.StartTicks;

        if (totalTicks <= 0)
        {
            return 1f;
        }

        long elapsedTicks = GameManager.Time.UtcNow.Ticks - slot.StartTicks;

        if (elapsedTicks <= 0)
        {
            return 0f;
        }

        if (elapsedTicks >= totalTicks)
        {
            return 1f;
        }

        return (float)elapsedTicks / totalTicks;
    }

    private static bool IsValidIndex(int index)
    {
        return 0 <= index && index < Slots.Count;
    }

    public static int BaseSlotCount
    {
        get
        {
            return BASE_SLOT_COUNT;
        }
    }

#if UNITY_EDITOR
    public static void DebugShiftSchedule(long shiftTicks)
    {
        List<AutoWorkSlot> slots = Slots;

        for (int i = 0; i < slots.Count; i++)
        {
            AutoWorkSlot slot = slots[i];

            slot.StartTicks += shiftTicks;
            slot.EndTicks += shiftTicks;

            slots[i] = slot;
        }

        NotifyQueueChanged();
    }

    public static void DebugCompleteFirst()
    {
        List<AutoWorkSlot> slots = Slots;

        if (slots.Count == 0)
        {
            return;
        }

        AutoWorkSlot slot = slots[0];
        slot.EndTicks = GameManager.Time.UtcNow.Ticks;
        slots[0] = slot;

        NotifyQueueChanged();
    }
#endif
}
