using System;

public class NetworkManager
{
    public CompanionApi CompanionService { get; private set; }

    public NetworkManager()
    {
        CompanionService = new CompanionApi();
    }

    public GameSaveData LoadAsync()
    {
        return new GameSaveData();
    }
    public void SaveAsync(GameSaveData saveData)
    {

    }
}

// 임시 TODO(김익환): 삭제하기
[Serializable]
public class GameSaveData
{

}