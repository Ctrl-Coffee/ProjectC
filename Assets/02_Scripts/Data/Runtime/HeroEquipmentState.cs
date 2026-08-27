

public class HeroEquipmentState
{
    public string HeroEquipmentId;
    public int Level;

    public HeroEquipmentState(EquipmentDto equipmentDto)
    {
        HeroEquipmentId = equipmentDto.equipmentId;
        Level = equipmentDto.level;
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
