
public abstract class ViewBase<T> : UIBase where T : ViewModelBase
{
    protected T _viewModel;

    protected void BindViewModel(T viewModel)
    {
        if(_viewModel != null)
        {
            UnSubscribe();
        }

        _viewModel = viewModel;

        _viewModel.InitializeModel();
    }

    protected virtual void Subscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        }
    }

    protected virtual void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
        }
    }

    protected abstract void OnPropertyChanged(string propertyName);
}
