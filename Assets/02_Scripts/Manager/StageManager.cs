using System.Collections.Generic;

public class StageManager
{
    private StageModel _stageModel = new StageModel();

    public StageModel StageModel { get { return _stageModel; } }

    public string StageName
    {
        get { return GetStageName(); }
    }

    public IReadOnlyList<string> EnemyGroupIds
    {
        get { return _stageModel.EnemyGroupIds; }
    }

    public float EnemyStatMultiplier
    {
        get { return _stageModel.EnemyStatMultiplier; }
    }

    public string NextStageId
    {
        get { return _stageModel.NextStageId; }
    }

    public string SpriteAddressableKey
    {
        get { return _stageModel.SpriteAddressableKey; }
    }

    public int DreamShardReward
    {
        get { return _stageModel.DreamShardReward; }
    }

    public int InspirationReward
    {
        get { return _stageModel.InspirationReward; }
    }

    public void SetStage(string stageId)
    {
        _stageModel.SetStage(stageId);
    }

    public void SetNextStage()
    {
        if (_stageModel.NextStageId == null)
        {
            Logger.Log("다음 스테이지가 없습니다.");
            return;
        }

        _stageModel.SetStage(_stageModel.NextStageId);
    }

    private string GetStageName()
    {
        string stageName = $"{_stageModel.Chapter}-{_stageModel.StageNumber}";
        return stageName;
    }
}
