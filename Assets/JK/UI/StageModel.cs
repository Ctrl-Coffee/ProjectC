using System;
using UnityEngine;

public class StageModel : ModelBase
{
    private string _stageId;
    private int _chapter;
    private int _stageNumber;
    private int _isBoss;
    private int _recommendedPlayerLevel;
    private float _enemyStatMultiplier;
    private readonly string[] _enemyGroupIds = new string[3];
    private int _dreamShardReward;
    private int _inspirationReward;
    private int _dpCost;

    public int Chapter
    {
        get { return _chapter; }
    }

    public int StageNumber
    {
        get { return _stageNumber; }
    }

    public int IsBoss
    {
        get { return _isBoss; }
    }

    public void SetStage(string stageId)
    {
        StageData stageData = GameManager.DataTable.GetStageData(stageId);

        if (stageData == null)
        {
            Debug.LogError($"존재하지 않는 StageId입니다. StageId: {stageId}");

            Clear();
            return;
        }

        SetStageData(stageData);

        InitializeOnce();
    }

    private void SetStageData(StageData stageData)
    {
        _stageId = stageData.Id;
        _chapter = stageData.Chapter;
        _stageNumber = stageData.StageNumber;
        _isBoss = stageData.IsBoss;
        _recommendedPlayerLevel = stageData.RecommendedPlayerLevel;
        _enemyStatMultiplier = stageData.EnemyStatMultiplier;
        _dreamShardReward = stageData.DreamShardReward;
        _inspirationReward = stageData.InspirationReward;
        _dpCost = stageData.DPCost;

        SetEnemyGroupIds(stageData.EnemyGroupId);
    }

    private void Clear()
    {
        _stageId = null;
        _chapter = 0;
        _stageNumber = 0;
        _isBoss = 0;
        _recommendedPlayerLevel = 0;
        _enemyStatMultiplier = 0f;
        _dreamShardReward = 0;
        _inspirationReward = 0;
        _dpCost = 0;

        Array.Clear(_enemyGroupIds, 0, _enemyGroupIds.Length);
    }

    private void SetEnemyGroupIds(string enemyGroupId)
    {
        string[] enemyGroupIds = enemyGroupId.Split(',');

        if (_enemyGroupIds.Length != enemyGroupIds.Length)
        {
            Debug.LogError("EnemyGroupId 개수가 잘못되었습니다.");
            Array.Clear(_enemyGroupIds, 0, _enemyGroupIds.Length);
            return;
        }

        for (int i = 0; i < _enemyGroupIds.Length; i++)
        {
            string enemyId = enemyGroupIds[i].Trim();
            _enemyGroupIds[i] = enemyId;
        }
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Chapter));
        OnPropertyChanged(nameof(StageNumber));
    }
}
