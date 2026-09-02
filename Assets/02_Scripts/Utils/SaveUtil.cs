using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins;
using System.Diagnostics;

public static class SaveUtil
{
    private static readonly SaveRequest _currencySaveRequest = new(SaveCurrencyAsync);
    private static readonly SaveRequest _companionSaveRequest = new(SaveCompanionAsync);
    private static readonly SaveRequest _equipmentSaveRequest = new(SaveEquipmentAsync);
    private static readonly SaveRequest _equipmentLoadoutSaveRequest = new(SaveEquipmentLoadoutAsync);
    private static readonly SaveRequest _heroLevelSaveRequest = new(SaveHeroLevelAsync);
    private static readonly SaveRequest _perkSaveRequest = new(SavePerkAsync);
    private static readonly SaveRequest _autoWorkSaveRequest = new(SaveAutoWorkAsync);
    private static readonly SaveRequest _companionPartySaveRequest = new(SaveCompanionPartyAsync);

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

    public static async UniTask<SaveStageRecordResponse> RequestSaveStageData(string stageId)
    {
        return await GameManager.Network.SaveStageAsync(stageId);
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

    public static void RequestSaveCompanionPartyData()
    {
        _companionPartySaveRequest.Request();
    }


    public static async UniTask SaveAllDataAsync()
    {
        await UniTask.WhenAll(SaveCurrencyAsync(), SaveAutoWorkAsync(), SaveStageAsync());
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

    private static async UniTask SaveCompanionPartyAsync()
    {
        await GameManager.Network.SaveCompanionPartyAsync(GameManager.Battle.CompanionFormationIds);
    }

    private static async UniTask SaveStageAsync()
    {
        string stageId = GameManager.Stage.LastClearedStageId;

        if (string.IsNullOrEmpty(stageId))
        {
            return;
        }

        SaveStageRecordResponse response = await GameManager.Network.SaveStageAsync(stageId);

        if (response.result != (int)ServerErrorCode.Success)
        {
            Logger.LogWarning($"스테이지 저장 실패: {response.message}");
        }
    }
}
