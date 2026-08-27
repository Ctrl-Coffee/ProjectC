using Cysharp.Threading.Tasks;

public class GameSession
{
    public CurrencyModel Currency { get; private set; }
    public CompanionModel Companion { get; private set; }
    public HeroEquipmentModel HeroEquipment { get; private set; }
    public HeroEquipedModel HeroEquiped { get; private set; }
    public GachaModel Gacha { get; private set; }


    public PlayerGrowthModel PlayerGrowth { get; }
    public HeroInfoModel HeroInfo { get; private set; }




    private NetworkManager _networkManager;


    public GameSession(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        PlayerGrowth = new(new());
        Gacha = new();
    }

    public async UniTask LoadAllData()
    {
        var currencyData = await _networkManager.LoadCurrencyAsync();
        Currency = new(currencyData.data);

        var companionData = await _networkManager.LoadCompanionAsync();
        CompanionWrapperDto companionWwrapperDto =  companionData.data;
        Companion = new(companionWwrapperDto.companions);

        var equipmentData = await _networkManager.LoadEquipmentAsync();
        EquipmentWrapperDto equipmentWwrapperDto = equipmentData.data;
        HeroEquipment = new(equipmentWwrapperDto.equipments);

        var equipmentLoadoutData = await _networkManager.LoadEquipmentLoadoutAsync();
        EquipmentLoadoutDto equipmentLoadoutDto = equipmentLoadoutData.data;
        HeroEquiped = new(equipmentLoadoutDto);

        HeroInfo = new(PlayerGrowth, HeroEquiped, HeroEquipment);
    }
}
