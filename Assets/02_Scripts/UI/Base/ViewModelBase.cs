using System;
using System.ComponentModel;

public abstract class ViewModelBase
{
    public event Action<string> OnPropertyChanged_ViewModel;

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged_ViewModel?.Invoke(e.PropertyName);
    }

    public abstract void InitializeModel();

    public abstract void UnBind();
}


public class ViewModelBase<T> : ViewModelBase where T : ModelBase
{
    protected T _model { get; private set; }

    public ViewModelBase(T model)
    {
        _model = model;
        _model.PropertyChanged += OnPropertyChanged;
    }

    public override void UnBind()
    {
        _model.PropertyChanged -= OnPropertyChanged;
        _model = null;
    }

    public override void InitializeModel()
    {
        _model.InitializeOnce();
    }
}
