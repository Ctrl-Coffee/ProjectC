using UnityEngine;

public class GrowthSystem
{
    public CharacterStatData GetFinalStat(OwnedCompanionData ownedCompanion)
    {
        CompanionData companion = GameManager.DataTable.GetCompanionData(ownedCompanion.CompanionId);

        if (companion == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id: {ownedCompanion.CompanionId}");
            return null;
        }

        float growthMultiplier = (ownedCompanion.Level - 1);
        float finalAttack = companion.BaseAtk + (companion.GrowthAtk * growthMultiplier);
        float finalDef = companion.BaseDef + (companion.GrowthDef * growthMultiplier);
        float finalHp = companion.BaseHp + (companion.GrowthHp * growthMultiplier);

        CharacterStatData finalStatData = new CharacterStatData(finalAttack, finalDef, finalHp);
        
        return finalStatData;
    }
}
