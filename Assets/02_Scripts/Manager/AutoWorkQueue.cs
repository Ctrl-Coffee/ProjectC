using System;
using System.Collections.Generic;

public static class AutoWorkQueue
{
    public const int MAX_SLOT_COUNT = 5;

    private static List<AutoWorkSlot> Slots
    {
        get
        {
            return GameManager.User.AutoWorkSlots;
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
            return Slots.Count < MAX_SLOT_COUNT;
        }
    }

    public static bool TryEnqueue(string workId)
    {
        if (!CanEnqueue)
        {
            Logger.LogWarning($"업무 큐가 가득 찼습니다. (최대 {MAX_SLOT_COUNT}개)");
            return false;
        }

        AutoWorkData data = GameManager.DataTable.GetAutoWorkData(workId);

        if (null == data)
        {
            Logger.LogError($"자동업무 데이터를 찾을 수 없습니다. id: {workId}");
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
            EndTicks = startTicks + (long)(data.DurationSeconds * TimeSpan.TicksPerSecond),
        });

        GameManager.Save.Save();

        return true;
    }

    public static bool TryCancel(int index)
    {
        if (!IsValidIndex(index))
        {
            return false;
        }

        Slots.RemoveAt(index);
        RecalculateFrom(index);

        GameManager.Save.Save();

        return true;
    }

    private static void RecalculateFrom(int startIndex)
    {
        List<AutoWorkSlot> slots = Slots;
        long nowTicks = GameManager.Time.UtcNow.Ticks;

        for (int i = startIndex; i < slots.Count; i++)
        {
            AutoWorkData data = GameManager.DataTable.GetAutoWorkData(slots[i].WorkId);

            if (null == data)
            {
                Logger.LogError($"자동업무 데이터를 찾을 수 없어 시각 재계산을 건너뜁니다. id: {slots[i].WorkId}");
                continue;
            }

            long startTicks = nowTicks;

            if (i > 0)
            {
                startTicks = Math.Max(nowTicks, slots[i - 1].EndTicks);
            }

            AutoWorkSlot slot = slots[i];

            slot.StartTicks = startTicks;
            slot.EndTicks = startTicks + (long)(data.DurationSeconds * TimeSpan.TicksPerSecond);

            slots[i] = slot;
        }
    }

    public static int CollectCompleted()
    {
        List<AutoWorkSlot> slots = Slots;
        long nowTicks = GameManager.Time.UtcNow.Ticks;
        int collectedCount = 0;

        while (slots.Count > 0 && nowTicks >= slots[0].EndTicks)
        {
            AutoWorkData data = GameManager.DataTable.GetAutoWorkData(slots[0].WorkId);

            slots.RemoveAt(0);
            collectedCount++;

            if (null == data)
            {
                Logger.LogError("정산할 자동업무 데이터를 찾을 수 없어 보상을 건너뜁니다.");
                continue;
            }

            GameManager.User.Currency.AddMoney(data.RewardMoney);
            GameManager.User.Currency.AddDP(data.RewardDP);

            Logger.Log($"자동업무 완료 - {data.Name} / 돈 {data.RewardMoney} / DP {data.RewardDP}");
        }

        if (collectedCount > 0)
        {
            GameManager.Save.Save();
        }

        return collectedCount;
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

#if UNITY_EDITOR
    // 즉시 완료
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
    }
#endif
}
