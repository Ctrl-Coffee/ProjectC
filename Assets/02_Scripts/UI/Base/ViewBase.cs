
public abstract class ViewBase : UIBase
{
    protected abstract void BindViewModel();
    protected abstract void Subscribe();
    protected abstract void UnSubscribe();
    protected abstract void OnPropertyChanged(string propertyName);
}
