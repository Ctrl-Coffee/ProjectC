using System.Collections.Generic;

public class StageManager
{
    private StageModel _stageModel = new StageModel();

    public StageModel StageModel { get { return _stageModel; } }

    public IReadOnlyList<string> EnemyGroupIds
    {
        get { return _stageModel.EnemyGroupIds; }
    }

    public string NextStageId
    {
        get { return _stageModel.NextStageId; }
    }

    public string SpriteAddressableKey
    {
        get { return _stageModel.SpriteAddressableKey; }
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
}
