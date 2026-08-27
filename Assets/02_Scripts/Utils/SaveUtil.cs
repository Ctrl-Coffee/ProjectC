using Cysharp.Threading.Tasks;

public static class SaveUtil
{
    private static readonly SaveRequest _currencySaveRequest = new(SaveCurrencyAsync);
    private static readonly SaveRequest _companionSaveRequest = new(SaveCompanionAsync);
    private static readonly SaveRequest _equipmentSaveRequest = new(SaveEquipmentAsync);
    private static readonly SaveRequest _equipmentLoadoutSaveRequest = new(SaveEquipmentLoadoutAsync);

    public static void RequestSaveCurrency()
    {
        _currencySaveRequest.Request();
    }

    public static void RequestSaveCompanion()
    {
        _companionSaveRequest.Request();
    }

    public static void RequestSaveEquipment()
    {
        _equipmentSaveRequest.Request();
    }

    public static void RequestSaveEquipmentLoadout()
    {
        _equipmentLoadoutSaveRequest.Request();
    }

    public static async UniTask SaveAllDataAsync()
    {
        await UniTask.WhenAll(SaveCurrencyAsync(), SaveEquipmentLoadoutAsync());
    }






    private static async UniTask SaveCurrencyAsync()
    {
        await GameManager.Network.SaveCurrencyAsync(GameManager.Session.Currency);
    }

    private static async UniTask SaveEquipmentAsync()
    {
        await GameManager.Network.SaveEquipmentAsync(GameManager.Session.HeroEquipment);
    }

    private static async UniTask SaveEquipmentLoadoutAsync()
    {
        await GameManager.Network.SaveEquipmentLoadoutAsync(GameManager.Session.HeroEquiped);
    }

    private static async UniTask SaveCompanionAsync()
    {
        await GameManager.Network.SaveCompanionAsync(GameManager.Session.Companion);
    }
}
