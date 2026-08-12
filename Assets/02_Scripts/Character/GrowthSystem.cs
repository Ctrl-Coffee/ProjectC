using System.Linq;
using UnityEngine;

public class GrowthSystem
{
    public CharacterStatData GetFinalStat(string companionId, int level )
    {
        CompanionData companion = GameManager.DataTable.GetCompanionData(companionId);

        if (companion == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id: {companionId}");
            return null;
        }

        float growthMultiplier = (level - 1);
        float finalAttack = companion.BaseAtk + (companion.GrowthAtk * growthMultiplier);
        float finalDef = companion.BaseDef + (companion.GrowthDef * growthMultiplier);
        float finalHp = companion.BaseHp + (companion.GrowthHp * growthMultiplier);

        CharacterStatData finalStatData = new CharacterStatData(finalAttack, finalDef, finalHp);
        
        return finalStatData;
    }

    public bool CheckCanLevelUp(string companionId, int level)
    {
        CompanionData companion = GameManager.DataTable.GetCompanionData(companionId);

        if (companion == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id: {companionId}");
            return false;
        }

        CompanionLevelUpCostData costData = GameManager.DataTable.GetCompanionLevelUpCost(companion.GradeStar.ToString());

        if (costData == null)
        {
            Debug.LogError($"레벨업 비용 데이터를 찾을 수 없습니다. 등급 : {companion.GradeStar}");
            return false;
        }

        if (level >= costData.LevelUpCost.Count + 1) return false;

        //TODO 희준 : 꿈의 조각 확인, 차감

        return true;
    }
}
