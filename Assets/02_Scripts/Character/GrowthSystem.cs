using UnityEngine;

public class GrowthSystem
{
    public bool IsPlayerMaxLevel(int level)
    {
        return GameManager.DataTable.GetPlayerLevelData(level + 1) == null;
    }

    public bool CheckPlayerCanLevelUp(int level)
    {
        if (IsPlayerMaxLevel(level))
        {
            return false;
        }

        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        return GameManager.Session.Currency.DreamFragment >= (long)nextLevelData.UpgradeCost; 
    }

    public bool TryPayPlayerLevelUpCost(int level)
    {
        if(CheckPlayerCanLevelUp(level) == false)
        {
            return false;
        }

        PlayerLevelData nextLevelData = GameManager.DataTable.GetPlayerLevelData(level + 1);

        return GameManager.Session.Currency.TrySpendDreamFragment((long)nextLevelData.UpgradeCost);
    }
}
