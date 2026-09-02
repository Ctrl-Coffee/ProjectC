using System;

[Serializable]
public class StageData : BaseData
{
    public int Chapter;
    public int StageNumber;
    public int IsBoss;
    public int RecommendedPlayerLevel;
    public float EnemyStatMultiplier;
    public string EnemyGroupId;
    public int DreamShardReward;
    public int InspirationReward;
    public int DPCost;
    public string NextStageId;
    public string SpriteAddressableKey;
    public string BgmAddressableKey;
}
