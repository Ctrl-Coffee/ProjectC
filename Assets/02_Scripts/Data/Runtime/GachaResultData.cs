public class GachaResultData
{
    public string Id;
    public GachaType GachaType;
    public bool IsDuplicate;
    public int DuplicateReward;

    public GachaResultData(string id, GachaType type, bool isDuplicate, int duplicateReward)
    {
        Id = id;
        GachaType = type;
        IsDuplicate = isDuplicate;
        DuplicateReward = duplicateReward;
    }
}
