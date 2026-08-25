using Cysharp.Threading.Tasks;
using System.Collections.Generic;

public class GameSession
{
    public CurrencyModel Currency { get; private set; }
    public PlayerGrowthModel PlayerGrowth { get; }

    public CompanionModel Companion { get; private set; }
    public HeroEquipmentModel HeroEquipment { get; private set; }
    public HeroEquipedModel HeroEquiped { get; private set; }


    private NetworkManager _networkManager;


    public GameSession(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        PlayerGrowth = new(new());

        List<HeroEquipmentState> heroEquipmentStates = new()
        {
            new("Equipment_001", 1), new("Equipment_002", 2), new("Equipment_003", 3)
            , new("Equipment_004", 4), new("Equipment_005", 5), new("Equipment_006", 6)
            , new("Equipment_007", 7), new("Equipment_008", 8), new("Equipment_009", 9)
            , new("Equipment_010", 10)
        };
        HeroEquipment = new(heroEquipmentStates);

        HeroEquiped = new();
    }

    public async UniTask LoadAllData()
    {
        var currencyData = await _networkManager.LoadCurrencyAsync();
        Currency = new(currencyData.data);

        var companionData = await _networkManager.LoadCompanionAsync();
        CompanionWrapperDto wrapperDto =  companionData.data;
        Companion = new(wrapperDto.companions);


    }
}
