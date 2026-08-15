public class CompanionState
{
    public string CompanionId { get; }
    public int Level { get; private set; }

    public CompanionState(string companionId, int level)
    {
        CompanionId = companionId;
        Level = level;
    }
}
