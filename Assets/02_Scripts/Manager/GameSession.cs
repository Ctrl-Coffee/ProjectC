using Cysharp.Threading.Tasks;

public class GameSession
{
    public CurrencyModel Currency { get; private set; }
    public CompanionModel Companion { get; private set; }
    public HeroEquipmentModel HeroEquipment { get; private set; }
    public HeroEquipedModel HeroEquiped { get; private set; }
    public GachaModel Gacha { get; private set; }
    public HeroInfoModel HeroInfo { get; private set; }



    private NetworkManager _networkManager;


    public GameSession(NetworkManager networkManager)
    {
        _networkManager = networkManager;

        Gacha = new();
    }

    public async UniTask LoadAllData()
    {
        var currencyResponse = await _networkManager.LoadCurrencyAsync();
        Currency = new(currencyResponse.data);

        var companionResponse = await _networkManager.LoadCompanionAsync();
        CompanionWrapperDto companionWwrapperDto =  companionResponse.data;
        Companion = new(companionWwrapperDto.companions);

        var equipmentResponse = await _networkManager.LoadEquipmentAsync();
        EquipmentWrapperDto equipmentWwrapperDto = equipmentResponse.data;
        HeroEquipment = new(equipmentWwrapperDto.equipments);

        var equipmentLoadoutResponse = await _networkManager.LoadEquipmentLoadoutAsync();
        EquipmentLoadoutDto equipmentLoadoutDto = equipmentLoadoutResponse.data;
        HeroEquiped = new(equipmentLoadoutDto, HeroEquipment);

        if (null != HeroInfo)
        {
            HeroInfo.Dispose();
        }

        var heroLevelResponse = await _networkManager.LoadHeroLevelAsync();
        var profileResponse = await _networkManager.LoadProfileAsync();

        int userLevel = Const.FIRST_LEVEL;

        if ((int)ServerErrorCode.Success == heroLevelResponse.result)
        {
            userLevel = heroLevelResponse.data.userLevel;
        }
        else
        {
            Logger.LogWarning($"주인공 레벨을 받지 못해 최소 레벨로 시작합니다. result : {heroLevelResponse.result}, message : {heroLevelResponse.message}");
        }

        HeroInfo = new(HeroEquiped, HeroEquipment, userLevel, profileResponse.nickname);
    }
}
