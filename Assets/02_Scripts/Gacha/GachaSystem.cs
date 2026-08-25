using System.Collections.Generic;
using UnityEngine;

public class GachaSystem
{
    //TODO 희준 : 기획 확정후 데이터 표로 이동
    private const long SINGLE_COST = 30;
    public const int MULTI_DRAW_COUNT = 5;

    public long GetDrawCost(int count)
    {
        return SINGLE_COST * count;
    }
    public bool CheckCanDraw(int count)
    {
        if (count <= 0)
        {
            return false;
        }

        return GameManager.Session.Currency.DreamScroll >= GetDrawCost(count);
    }

    public bool TryPayDrawCost(int count)
    {
        if (CheckCanDraw(count) == false)
        {
            return false;
        }

        return GameManager.Session.Currency.TrySpendDreamScroll(GetDrawCost(count));
    }

    public IReadOnlyList<string> Draw(GachaType gachaType, int count)
    {
        IReadOnlyList<GachaProbabilityData> probabilities = GameManager.DataTable.GetGachaProbabilityData(gachaType);

        if (probabilities == null) return null;

        List<string> results = new List<string>();

        for (int i = 0; i < count; i++)
        {
            GachaProbabilityData picked = DrawGrade(probabilities);

            if (picked == null) continue;

            IReadOnlyList<string> candidates = GetCandidateIds(gachaType, picked.Grade);

            if (candidates == null || candidates.Count ==0) continue;

            results.Add(candidates[Random.Range(0, candidates.Count)]);
        }

        return results;
    }

    private GachaProbabilityData DrawGrade(IReadOnlyList<GachaProbabilityData> probabilities)
    {
        int totalWeight = 0;

        foreach (GachaProbabilityData data in probabilities)
        {
            totalWeight += data.Probability;
        }

        if (totalWeight <= 0)
        {
            Logger.LogError("확률 가중치의 합이 0 이하입니다");
            return null;
        }

        int randomValue = Random.Range(0, totalWeight);
        int cursor = 0;

        foreach (GachaProbabilityData data in probabilities)
        {
            cursor += data.Probability;

            if (randomValue < cursor)
            {
                return data;
            }
        }

        return probabilities[probabilities.Count - 1];
    }

    private IReadOnlyList<string> GetCandidateIds(GachaType gachaType, int grade)
    {
        switch (gachaType)
        {
            case GachaType.Companion:
                {
                    IReadOnlyList<CompanionData> companions = GameManager.DataTable.GetCompanionsByGrade(grade);

                    if (companions == null) return null;

                    List<string> ids = new List<string>();

                    foreach (CompanionData data in companions)
                    {
                        ids.Add(data.Id);
                    }

                    return ids;
                }

            case GachaType.Equipment:
                { 
                    IReadOnlyList<EquipmentData> equipment = GameManager.DataTable.GetEquipmentsByGrade(grade);

                    if (equipment == null) return null;

                    List<string> ids = new List<string>();

                    foreach (EquipmentData data in equipment)
                    {
                        ids.Add(data.Id);
                    }

                        return ids;
                    }

            default:
                Logger.LogError($"구현되지 않은 가차 종류입니다 {gachaType}");
                return null;
        }
    }

    public int GetDuplicateReward(GachaType gachaType, int grade)
    {
        IReadOnlyList<GachaProbabilityData> probabilities = GameManager.DataTable.GetGachaProbabilityData(gachaType);

        if (probabilities == null) return 0;

        foreach (GachaProbabilityData data in probabilities)
        {
            if (data.Grade == grade)
            {
                return data.DuplicateReward;
            }
        }

        Logger.LogError($"중복 복상 데이터를 찾을수 없습니다. 종류 {gachaType}, 등급 {grade}");
        return 0;
    }
}
