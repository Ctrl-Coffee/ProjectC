using System.Collections.Generic;

[System.Serializable]
public class EquipmentWrapperDto
{
    public List<EquipmentDto> equipments { get; set; } = new();
}
