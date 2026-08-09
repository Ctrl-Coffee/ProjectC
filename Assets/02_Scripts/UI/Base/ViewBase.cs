
public abstract class ViewBase<T> : UIBase where T : ViewModelBase
{
    protected T _viewModel;

    public void BindViewModel(T viewModel)
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }

        _viewModel = viewModel;
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _viewModel.InitializeModel();
    }

    protected virtual void OnDestroy()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected abstract void OnPropertyChanged(string propertyName);
}
