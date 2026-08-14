public class EquipmentGrowthModel : ModelBase
{
    private OwnedEquipmentData _ownedEquipmentData;

    public int Level
    {
        get { return _ownedEquipmentData.Level; }
        set
        {
            if (_ownedEquipmentData.Level == value) return;
            _ownedEquipmentData.Level = value;
            OnPropertyChanged();
        }
    }

    public string EquipmentId { get { return _ownedEquipmentData.EquipmentId; } }

    public EquipmentGrowthModel(OwnedEquipmentData ownedEquipmentData)
    {
        _ownedEquipmentData = ownedEquipmentData;
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged((nameof(Level)));
    }
}

