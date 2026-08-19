public class GachaResultData
{
    public string Id;
    public bool IsDuplicate;
    public int DuplicateReward;

    public GachaResultData(string id, bool isDuplicate, int duplicateReward)
    {
        Id = id;
        IsDuplicate = isDuplicate;
        DuplicateReward = duplicateReward;
    }
}
