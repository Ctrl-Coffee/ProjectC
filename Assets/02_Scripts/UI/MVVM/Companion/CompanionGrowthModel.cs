public class CompanionGrowthModel : ModelBase
{
    private OwnedCompanionData _ownedCompanionData;
    public int Level
    {
        get {return _ownedCompanionData.Level;}
        set
        {
            if (_ownedCompanionData.Level == value) return;
            _ownedCompanionData.Level = value;
            OnPropertyChanged();
        }
    }

    public string CompanionId{ get { return _ownedCompanionData.CompanionId; } }

    public CompanionGrowthModel(OwnedCompanionData ownedCompanionData)
    {
        _ownedCompanionData = ownedCompanionData;
    }
    public override void InitializeOnce()
    {
        OnPropertyChanged((nameof(Level)));
    }
}
