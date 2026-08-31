
public class HeroEquipedModel : ModelBase
{
    public HeroEquipmentState EquipedWeapon => _equipedWeapon;
    public HeroEquipmentState EquipedArmor => _equipedArmor;
    public HeroEquipmentState EquipedAccessory => _equipedAccessory;

    private HeroEquipmentModel _equipmentModel;

    private ConfirmData _confirmData;

    private HeroEquipmentState _equipedWeapon;
    private HeroEquipmentState _equipedArmor;
    private HeroEquipmentState _equipedAccessory;

    public HeroEquipedModel(EquipmentLoadoutDto equipmentLoadoutDto, HeroEquipmentModel heroEquipmentModel)
    {
        _equipmentModel = heroEquipmentModel; 

        EquipedWeaponId = equipmentLoadoutDto.weaponEquipmentId;
        EquipedArmorId = equipmentLoadoutDto.armorEquipmentId;
        EquipedAccessoryId = equipmentLoadoutDto.accessoryEquipmentId;

        SetDefaultEquipment();

        InitializeOnce();

        _confirmData = GameManager.DataTable.GetConfirmData(Const.DEFAULT_EQUIPMENT);
    }

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
                {
                    EquipedWeaponId = equipmentId;
                    _equipedWeapon = _equipmentModel.GetHeroEquipment(equipmentId);
                }
                break;
            case EquipmentType.Armor:
                {
                    EquipedArmorId = equipmentId;
                    _equipedArmor = _equipmentModel.GetHeroEquipment(equipmentId);
                }
                break;
            case EquipmentType.Accessory:
                {
                    EquipedAccessoryId = equipmentId;
                    _equipedAccessory = _equipmentModel.GetHeroEquipment(equipmentId);
                }
                break;
        }

        SaveUtil.RequestSaveEquipmentLoadout();
    }

    public void UnEquip(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Weapon:
                {
                    if(_euipedWeaponId == Const.DEFAULT_WEAPON_EQUIPMENT_ID)
                    {
                        GameManager.UI.OpenConfirmUI(_confirmData);
                        return;
                    }
                    Equip(EquipmentType.Weapon, Const.DEFAULT_WEAPON_EQUIPMENT_ID);
                }
                break;
            case EquipmentType.Armor:
                {
                    if (_euipedArmorId == Const.DEFAULT_ARMOR_EQUIPMENT_ID)
                    {
                        GameManager.UI.OpenConfirmUI(_confirmData);
                        return;
                    }
                    Equip(EquipmentType.Armor, Const.DEFAULT_ARMOR_EQUIPMENT_ID);
                }
                break;
            case EquipmentType.Accessory:
                {
                    if (_euipedAccessoryId == Const.DEFAULT_ACCESSORY_EQUIPMENT_ID)
                    {
                        GameManager.UI.OpenConfirmUI(_confirmData);
                        return;
                    }
                    Equip(EquipmentType.Accessory, Const.DEFAULT_ACCESSORY_EQUIPMENT_ID);
                }
                break;
        }

        SaveUtil.RequestSaveEquipmentLoadout();
    }

    public string GetEquipedId(EquipmentType type)
    {
        return type switch
        {
            EquipmentType.Weapon => EquipedWeaponId,
            EquipmentType.Armor => EquipedArmorId,
            EquipmentType.Accessory => EquipedAccessoryId,
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

    private void SetDefaultEquipment()
    {
        if(EquipedWeaponId == null)
        {
            Equip(EquipmentType.Weapon, Const.DEFAULT_WEAPON_EQUIPMENT_ID);
        }

        if(EquipedArmorId == null)
        {
            Equip(EquipmentType.Armor, Const.DEFAULT_ARMOR_EQUIPMENT_ID);
        }

        if(EquipedAccessoryId == null)
        {
            Equip(EquipmentType.Accessory, Const.DEFAULT_ACCESSORY_EQUIPMENT_ID);
        }
    }
}