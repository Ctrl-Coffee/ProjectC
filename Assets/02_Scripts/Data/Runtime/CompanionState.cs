public class CompanionState
{
    public string CompanionId { get; }
    public int Level { get; private set; }

    public CompanionState(CompanionDto companionDto)
    {
        CompanionId = companionDto.companionId;
        Level = companionDto.level;
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
