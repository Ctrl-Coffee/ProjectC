using System.Collections.Generic;

public sealed class GameSession
{
    public CurrencyModel Currency { get; }
    public CompanionModel Companions { get; }
    public PlayerGrowthModel PlayerGrowth { get; }
    public EquipmentGrowthModel EquipmentGrowth { get; }


    public GameSession()
    {
        // 네트워크 매니저가 수신한 데이터를 받아 모델들을 생성.

        // GameSaveData 한번에 모든 데이터가 담겨 있지 않게 도메인 별로 나누자.
    }
}
