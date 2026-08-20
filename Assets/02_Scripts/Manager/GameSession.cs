using System.Collections.Generic;

public class GameSession
{
    public CurrencyModel Currency { get; }
    public PlayerGrowthModel PlayerGrowth { get; }

    public CompanionModel Companion { get; }
    public GachaModel Gacha { get; }
    public HeroEquipmentModel HeroEquipment { get; }
    public HeroEquipedModel HeroEquiped { get; }


    private NetworkManager _networkManager;


    public GameSession()
    {
        // 네트워크 매니저가 수신한 데이터를 받아 모델들을 생성.

        // GameSaveData 한번에 모든 데이터가 담겨 있지 않게 도메인 별로 나누자.

        // 임시
        Currency = new();

        PlayerGrowth = new(new());

        List<CompanionState> companionStates = new()
        {
            new("111", 1), new("112", 2), new("113", 3)
            , new("211", 2), new("212", 3), new("213", 4)
            , new("311", 3)
        };
        Companion = new(companionStates);
        Gacha = new();


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
}
