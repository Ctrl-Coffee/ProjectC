public class CompanionState
{
    public string CompanionId { get; }
    public int Level { get; private set; }

    public CompanionState(CompanionState other)
    {
        CompanionId = other.CompanionId;
        Level = other.Level;
    }

    public CompanionState(string companionId, int level)
    {
        CompanionId = companionId;
        Level = level;
    }

    public void LevelUp()
    {
        Level++;
    }
}
