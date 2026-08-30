using Cysharp.Threading.Tasks;

public static class SaveUtil
{
    private static readonly SaveRequest _currencySaveRequest = new(SaveCurrencyAsync);
    private static readonly SaveRequest _companionSaveRequest = new(SaveCompanionAsync);
    private static readonly SaveRequest _equipmentSaveRequest = new(SaveEquipmentAsync);
    private static readonly SaveRequest _equipmentLoadoutSaveRequest = new(SaveEquipmentLoadoutAsync);
    private static readonly SaveRequest _heroLevelSaveRequest = new(SaveHeroLevelAsync);
    private static readonly SaveRequest _perkSaveRequest = new(SavePerkAsync);
    private static readonly SaveRequest _autoWorkSaveRequest = new(SaveAutoWorkAsync);

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

    public static async UniTask RequestSaveStageData(int stage)
    {
        await GameManager.Network.SaveStageAsync(stage);
    }

    public static void RequestSaveHeroLevelData()
    {
        _heroLevelSaveRequest.Request();
    }

    public static void RequestSavePerkData()
    {
        _perkSaveRequest.Request();
    }

    public static void RequestSaveAutoWorkData()
    {
        _autoWorkSaveRequest.Request();
    }



    public static async UniTask SaveAllDataAsync()
    {
        await UniTask.WhenAll(SaveCurrencyAsync(), SaveAutoWorkAsync());
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

    private static async UniTask SaveHeroLevelAsync()
    {
        await GameManager.Network.SaveHeroLevelAsync(GameManager.Session.HeroInfo);
    }

    private static async UniTask SavePerkAsync()
    {
        await GameManager.Network.SavePerkAsync();
    }

    private static async UniTask SaveAutoWorkAsync()
    {
        await GameManager.Network.SaveAutoWorkSlotAsync();
    }
}
