using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

public class StageManager
{
    public StageModel StageModel { get { return _stageModel; } }
    public event Action OnStageProgressChanged;


    private readonly List<string> _stageIdsInOrder = new();
    private readonly Dictionary<string, int> _stageOrderById = new();

    private string _highestClearedStageId;

    private StageModel _stageModel = new StageModel();

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

    public string BgmAddressableKey
    {
        get { return _stageModel.BgmAddressableKey; }
    }

    public int DreamShardReward
    {
        get { return _stageModel.DreamShardReward; }
    }

    public int InspirationReward
    {
        get { return _stageModel.InspirationReward; }
    }

    public int DpCost
    {
        get { return _stageModel.DpCost; }
    }

    // 최고 클리어 기록
    public string HighestClearedStageId
    {
        get { return _highestClearedStageId; }
    }

    // 현재 입장 가능한 가장 높은 스테이지
    public string HighestUnlockedStageId
    {
        get
        {
            if (string.IsNullOrEmpty(_highestClearedStageId))
            {
                return _stageIdsInOrder[0];
            }

            int highestClearedOrder = _stageOrderById[_highestClearedStageId];

            int highestUnlockedOrder = Math.Min(highestClearedOrder + 1, _stageIdsInOrder.Count - 1);

            return _stageIdsInOrder[highestUnlockedOrder];
        }
    }

    // 현재 입장 가능한 가장 높은 챕터
    public int HighestUnlockedChapter
    {
        get
        {
            return GameManager.DataTable.GetStageData(HighestUnlockedStageId).Chapter;
        }
    }


    public string CurrentStageId
    {
        get { return _stageModel.StageId; }
    }

    public int Chapter
    {
        get { return _stageModel.Chapter; }
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

        if (string.IsNullOrEmpty(_highestClearedStageId))
        {
            return targetOrder == 0 ? StageState.Unlocked : StageState.Locked;
        }

        if (!_stageOrderById.TryGetValue(_highestClearedStageId, out int lastClearedOrder))
        {
            Logger.LogError($"클리어된 스테이지 순서를 찾을 수 없습니다. Id: {_highestClearedStageId}");
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

        if (string.IsNullOrEmpty(loadedStageId) || loadedStageId == "0")
        {
            _highestClearedStageId = null;
            return;
        }

        if (!_stageOrderById.ContainsKey(loadedStageId))
        {
            Logger.LogError($"서버에서 받은 최고 클리어 스테이지가 존재하지 않습니다. Id: {loadedStageId}");

            _highestClearedStageId = null;
            return;
        }

        _highestClearedStageId = loadedStageId;
    }

    public bool TryUpdateHighestClearedStage(string clearedStageId)
    {
        if (!_stageOrderById.TryGetValue(clearedStageId, out int clearedStageOrder))
        {
            Logger.LogError($"스테이지 순서를 찾을 수 없습니다. Id: {clearedStageId}");
            return false;
        }

        if (!string.IsNullOrEmpty(_highestClearedStageId))
        {
            int highestClearedStageOrder = _stageOrderById[_highestClearedStageId];

            if (clearedStageOrder <= highestClearedStageOrder)
            {
                return false;
            }
        }

        _highestClearedStageId = clearedStageId;
        OnStageProgressChanged?.Invoke();

        return true;
    }


    private string GetStageName()
    {
        string stageName = $"{_stageModel.Chapter}-{_stageModel.StageNumber}";
        return stageName;
    }
}
