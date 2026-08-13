public class PlayerGrowthModel : ModelBase
{
    private OwnedPlayerData _ownedPlayerData;

    public int Level
    {
        get { return _ownedPlayerData.Level; }
        set
        {
            if (_ownedPlayerData.Level == value) return;
            _ownedPlayerData.Level = value;
            OnPropertyChanged();
        }
    }

    public PlayerGrowthModel(OwnedPlayerData ownedPlayerData)
    {
        _ownedPlayerData = ownedPlayerData;
        if (_ownedPlayerData.Level <= 0)
        {
            _ownedPlayerData.Level = 1;
        }
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Level));
    }
}
