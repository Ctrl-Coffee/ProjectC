using System.Collections.Generic;

public class CompanionInventoryViewModel : ViewModelBase<CompanionModel>
{
    public event System.Action<string, ContainerPropertyChangedEvent, CompanionState> OnContainerChanged_ViewModel;

    public IReadOnlyList<CompanionState> Items => _items;
    public CompanionInventorySort CurrentSort => _currentSort;

    private CompanionInventorySort _currentSort = CompanionInventorySort.Level;
    private readonly List<CompanionState> _items = new();

    public CompanionInventoryViewModel(CompanionModel model) : base(model)
    {
        model.ContainerPropertyChanged += OnContainerChanged;
        RefreshItems();
    }

    public override void UnBind()
    {
        _model.ContainerPropertyChanged -= OnContainerChanged;
        base.UnBind();
    }

    public void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, CompanionState companionState)
    {
        switch (changedEvent)
        {
            case ContainerPropertyChangedEvent.Add:
                AddItem(companionState);
                break;

            case ContainerPropertyChangedEvent.Remove:
                RemoveItem(companionState);
                break;

            case ContainerPropertyChangedEvent.Update:
                UpdateItem(companionState);
                break;
        }

        OnContainerChanged_ViewModel?.Invoke(propertyName, changedEvent, companionState);
    }

    public void SetSort(int index)
    {
        _currentSort = (CompanionInventorySort)index;
        SortItems();
    }

    public int GetItemIndex(CompanionState value)
    {
        return _items.IndexOf(value);
    }

    public CompanionState GetCompanionState(string companionId)
    {
        return _model.GetCompanion(companionId);
    }

    private void SortItems()
    {
        if (_currentSort == CompanionInventorySort.Level)
            _items.Sort((x, y) =>
            {
                int levelCompare = y.Level.CompareTo(x.Level);

                if (levelCompare != 0)
                {
                    return levelCompare;
                }

                return string.CompareOrdinal(
                    x.CompanionId,
                    y.CompanionId);
            });
        else if(_currentSort == CompanionInventorySort.LevelReverse)
            _items.Sort((x, y) =>
            {
                int levelCompare = x.Level.CompareTo(y.Level);

                if (levelCompare != 0)
                {
                    return levelCompare;
                }

                return string.CompareOrdinal(
                    x.CompanionId,
                    y.CompanionId);
            });


    }

    private void RefreshItems()
    {
        _items.Clear();

        foreach (CompanionState companionState in _model.Companions.Values)
        {
            _items.Add(companionState);
        }

        SortItems();
    }

    private void AddItem(CompanionState companionState)
    {
        _items.Add(companionState);

        SortItems();
    }

    private void RemoveItem(CompanionState companionState)
    {
        _items.Remove(companionState);
    }

    private void UpdateItem(CompanionState companionState)
    {
        SortItems();
    }

    // TODO: 테스트
    public LevelUpResult TempLevelUp(string companionId)
    {
        return _model.TryLevelUp(companionId);
    }
}
