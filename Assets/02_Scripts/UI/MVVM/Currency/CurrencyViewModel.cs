public class CurrencyViewModel : ViewModelBase<CurrencyModel>
{
    public CurrencyViewModel(CurrencyModel model) : base(model)
    {
    }

    public long Money
    {
        get
        {
            return _model.Money;
        }
    }

    public long DreamPoint
    {
        get
        {
            return _model.DreamPoint;
        }
    }

    public long Energy
    {
        get
        {
            return _model.Energy;
        }
    }

    public long MaxEnergy
    {
        get
        {
            return _model.MaxEnergy;
        }
    }

    public long DreamFragment
    {
        get
        {
            return _model.DreamFragment;
        }
    }

    public long DreamScroll
    {
        get
        {
            return _model.DreamScroll;
        }
    }

    public long Inspiration
    {
        get
        {
            return _model.Inspiration;
        }
    }
}
