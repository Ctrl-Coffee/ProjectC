public class GachaModel : ModelBase
{
    private GachaType _gotchaType;
    public GachaType CurrentType 
    {
        get {  return _gotchaType; }
        set
        {
            if (_gotchaType == value) return;

            _gotchaType = value;
            OnPropertyChanged();
        }
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(CurrentType));
    }
}
