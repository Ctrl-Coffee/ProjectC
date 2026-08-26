using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SaveUtil
{
    public static async UniTask<SaveCurrencyResponse> SaveCurrencyAsync()
    {
        return await GameManager.Network.SaveCurrencyAsync(GameManager.Session.Currency);
    }

    public static async UniTask<SaveCompanionResponse> SaveCompanionAsync()
    {
        return await GameManager.Network.SaveCompanionAsync(GameManager.Session.Companion);
    }

    public static async UniTask<SaveEquipmentResponse> SaveEquipmentAsync()
    {
        return await GameManager.Network.SaveEquipmentAsync(GameManager.Session.HeroEquipment);
    }

    public static async UniTask<SaveEquipmentLoadoutResponse> SaveEquipmentLoadoutAsync()
    {
        return await GameManager.Network.SaveEquipmentLoadoutAsync(GameManager.Session.HeroEquiped);
    }
}
