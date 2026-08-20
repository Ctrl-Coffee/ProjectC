
public class HeroEquipedModel : ModelBase
{
    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(EquipedWeaponId));
        OnPropertyChanged(nameof(EquipedArmorId));
        OnPropertyChanged(nameof(EquipedAccessoryId));
    }

    public void Equip(EquipmentType type, string equipmentId)
    {
        switch(type)
        {
            case EquipmentType.Weapon:
                EquipedWeaponId = equipmentId;
                break;
            case EquipmentType.Armor:
                EquipedArmorId = equipmentId;
                break;
            case EquipmentType.Accessories:
                EquipedAccessoryId = equipmentId;
                break;
        }
    }

    public void UnEquip(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                EquipedWeaponId = null;
                break;
            case EquipmentType.Armor:
                EquipedArmorId = null;
                break;
            case EquipmentType.Accessories:
                EquipedAccessoryId = null;
                break;
        }
    }

    public string GetEquipedId(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.Weapon => EquipedWeaponId,
            EquipmentType.Armor => EquipedArmorId,
            EquipmentType.Accessories => EquipedAccessoryId,
            _ => null,
        };
    }

    private string _euipedWeaponId;
    public string EquipedWeaponId
    {
        get => _euipedWeaponId;
        set
        {
            if (_euipedWeaponId != value)
            {
                _euipedWeaponId = value;
                OnPropertyChanged(nameof(EquipedWeaponId));
            }
        }
    }
    private string _euipedArmorId;
    public string EquipedArmorId
    {
        get => _euipedArmorId;
        set
        {
            if (_euipedArmorId != value)
            {
                _euipedArmorId = value;
                OnPropertyChanged(nameof(EquipedArmorId));
            }
        }
    }
    private string _euipedAccessoryId;
    public string EquipedAccessoryId
    {
        get => _euipedAccessoryId;
        set
        {
            if (_euipedAccessoryId != value)
            {
                _euipedAccessoryId = value;
                OnPropertyChanged(nameof(EquipedAccessoryId));
            }
        }
    }
}
