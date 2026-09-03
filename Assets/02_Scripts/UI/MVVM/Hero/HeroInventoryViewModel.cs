using System.Collections.Generic;

public class HeroInventoryViewModel : ViewModelBase<HeroEquipmentModel>
{
    public event System.Action<string, ContainerPropertyChangedEvent, HeroEquipmentState> OnContainerChanged_ViewModel;

    public IReadOnlyList<HeroEquipmentState> Items => _items;
    public InventorySort CurrentSort => _currentSort;

    private InventorySort _currentSort = InventorySort.Level;
    private readonly List<HeroEquipmentState> _items = new();


    public HeroInventoryViewModel(HeroEquipmentModel model) : base(model)
    {
        model.ContainerPropertyChanged += OnContainerChanged;
        RefreshItems(); 
    }

    public override void UnBind()
    {
        _model.ContainerPropertyChanged -= OnContainerChanged;
        base.UnBind();
    }

    public void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, HeroEquipmentState state)
    {
        switch (changedEvent)
        {
            case ContainerPropertyChangedEvent.Add:
                AddItem(state);
                break;

            case ContainerPropertyChangedEvent.Remove:
                RemoveItem(state);
                break;

            case ContainerPropertyChangedEvent.Update:
                SortItems();
                break;
        }

        OnContainerChanged_ViewModel?.Invoke(propertyName, changedEvent, state);
    }

    public void SetSort(int index)
    {
        _currentSort = (InventorySort)index;
        SortItems();
    }

    public int GetItemIndex(HeroEquipmentState value)
    {
        return _items.IndexOf(value);
    }

    public HeroEquipmentState GetHeroEquipmentState(string id)
    {
        return _model.GetHeroEquipment(id);
    }

    public LevelUpResult TryLevelUp(string heroEquipmentId)
    {
        return _model.TryLevelUp(heroEquipmentId);
    }

    public int GetLevel(string heroEquipmentId)
    {
        return _model.GetLevel(heroEquipmentId);
    }

    private void SortItems()
    {
        if (_currentSort == InventorySort.Level)
            _items.Sort((x, y) =>
            {
                int levelCompare = y.Level.CompareTo(x.Level);

                if (levelCompare != 0)
                {
                    return levelCompare;
                }

                return string.CompareOrdinal(x.HeroEquipmentId, y.HeroEquipmentId);
            });
        else if (_currentSort == InventorySort.CombatPower)
            _items.Sort((x, y) =>
            {
                int combatPowerCompare = y.CombatPower.CompareTo(x.CombatPower);

                if (combatPowerCompare != 0)
                {
                    return combatPowerCompare;
                }

                return string.CompareOrdinal(x.HeroEquipmentId, y.HeroEquipmentId);
            });
    }

    private void RefreshItems()
    {
        _items.Clear();

        foreach (HeroEquipmentState state in _model.Equipments.Values)
        {
            _items.Add(state);
        }

        SortItems();
    }

    private void AddItem(HeroEquipmentState state)
    {
        _items.Add(state);

        SortItems();
    }

    private void RemoveItem(HeroEquipmentState state)
    {
        _items.Remove(state);
    }
}
