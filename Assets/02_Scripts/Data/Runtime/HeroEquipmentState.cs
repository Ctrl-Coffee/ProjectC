

public class HeroEquipmentState
{
    public string HeroEquipmentId;
    public int Level;

    public HeroEquipmentState(HeroEquipmentState other)
    {
        HeroEquipmentId = other.HeroEquipmentId;
        Level = other.Level;
    }

    public HeroEquipmentState(string id, int level)
    {
        HeroEquipmentId = id;
        Level = level;
    }

    public void LevelUp()
    {
        Level++;
    }
}
