
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

        OnBindViewModel();

        _viewModel.InitializeModel();
    }

    protected virtual void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected virtual void OnBindViewModel() { }
    protected abstract void OnPropertyChanged(string propertyName);
}
