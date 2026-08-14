using System;
using System.Collections.Generic;

public class ViewModelManager
{
    private Dictionary<Type, ViewModelBase> _viewModels = new();

    public void Register(ViewModelBase viewModel)
    {
        if (viewModel == null) return;

        Type type = viewModel.GetType();

        if (_viewModels.TryGetValue(type, out ViewModelBase oldViewModel))
        {
            oldViewModel.UnBind();
        }

        _viewModels[type] = viewModel;

    }
    public T Get<T>() where T : ViewModelBase
    {
        if (_viewModels.TryGetValue(typeof(T), out ViewModelBase viewModel))
        {
            return viewModel as T;
        }

        return null;
    }

    public void UnBind<T>() where T : ViewModelBase
    {
        Type type = typeof(T);

        if (_viewModels.TryGetValue(type, out ViewModelBase viewModel))
        {
            viewModel.UnBind();
            _viewModels.Remove(type);
        }
    }

    public void UnBindAll()
    {
        foreach (ViewModelBase viewModel in _viewModels.Values)
        {
            viewModel.UnBind();
        }

        _viewModels.Clear();
    }
}
