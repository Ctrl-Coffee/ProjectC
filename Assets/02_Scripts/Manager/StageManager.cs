using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class StageManager
{
    public StageModel StageModel { get { return _stageModel; } }
    public event Action OnStageProgressChanged;


    private readonly List<string> _stageIdsInOrder = new();
    private readonly Dictionary<string, int> _stageOrderById = new();

    private string _lastClearedStageId;

    private StageModel _stageModel = new StageModel();


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

    public string CurrentStageId
    {
        get { return _stageModel.StageId; }
    }

    public string LastClearedStageId
    {
        get { return _lastClearedStageId; }
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

    public bool IsHigherStage(string candidateStageId, string savedStageId)
    {
        StageData candidateStageData = GameManager.DataTable.GetStageData(candidateStageId);

        if (candidateStageData == null)
        {
            Logger.LogError($"스테이지 데이터를 찾을 수 없습니다. Id: {candidateStageId}");
            return false;
        }

        if (string.IsNullOrEmpty(savedStageId))
        {
            return true;
        }

        StageData savedStageData = GameManager.DataTable.GetStageData(savedStageId);

        if (savedStageData == null)
        {
            Logger.LogError($"저장된 스테이지 데이터를 찾을 수 없습니다. Id: {savedStageId}");
            return false;
        }

        if (candidateStageData.Chapter != savedStageData.Chapter)
        {
            return candidateStageData.Chapter > savedStageData.Chapter;
        }

        return candidateStageData.StageNumber > savedStageData.StageNumber;
    }

    public void Initialize()
    {
        List<StageData> stageDataList = new(GameManager.DataTable.StageDataTable.Values);

        stageDataList.Sort((left, right) =>
        {
            int chapterResult = left.Chapter.CompareTo(right.Chapter);

            if (chapterResult != 0)
            {
                return chapterResult;
            }

            return left.StageNumber.CompareTo(right.StageNumber);
        });

        for (int i = 0; i < stageDataList.Count; i++)
        {
            string stageId = stageDataList[i].Id;

            _stageIdsInOrder.Add(stageId);
            _stageOrderById.Add(stageId, i);
        }
    }

    public StageState GetStageState(string stageId)
    {
        if (!_stageOrderById.TryGetValue(stageId, out int targetOrder))
        {
            Logger.LogError($"스테이지 순서를 찾을 수 없습니다. Id: {stageId}");
            return StageState.Locked;
        }

        if (string.IsNullOrEmpty(_lastClearedStageId))
        {
            return targetOrder == 0 ? StageState.Unlocked : StageState.Locked;
        }

        if (!_stageOrderById.TryGetValue(_lastClearedStageId, out int lastClearedOrder))
        {
            Logger.LogError($"클리어된 스테이지 순서를 찾을 수 없습니다. Id: {_lastClearedStageId}");
            return StageState.Locked;
        }

        if (targetOrder <= lastClearedOrder)
        {
            return StageState.Cleared;
        }

        if (targetOrder == lastClearedOrder + 1)
        {
            return StageState.Unlocked;
        }

        return StageState.Locked;
    }

    public bool TrySetStage(string stageId)
    {
        if (GetStageState(stageId) == StageState.Locked)
        {
            Logger.Log($"잠긴 스테이지입니다. Id: {stageId}");
            return false;
        }

        _stageModel.SetStage(stageId);
        return true;
    }

    public async UniTask LoadDataAsync()
    {
        LoadStageRecordResponse response = await GameManager.Network.LoadStageAsync();

        string loadedStageId = response.data.lastClearedStage;

        _lastClearedStageId = loadedStageId == "0" ? null : loadedStageId;
    }

    public void SetLastClearedStageId(string stageId)
    {
        if (_lastClearedStageId == stageId)
        {
            return;
        }

        _lastClearedStageId = stageId;
        OnStageProgressChanged?.Invoke();
    }
}
