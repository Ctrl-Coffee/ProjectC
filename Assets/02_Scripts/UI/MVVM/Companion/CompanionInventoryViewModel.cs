using System;
using System.Collections.Generic;
using System.ComponentModel;

//public sealed class CompanionInventoryItem
//{
//    public string CompanionId { get; }
//    public string Name { get; }
//    public int Level { get; }

//    public CompanionInventoryItem(
//        string companionId,
//        string name,
//        int level)
//    {
//        CompanionId = companionId;
//        Name = name;
//        Level = level;
//    }
//}

public sealed class CompanionInventoryViewModel
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public float Attack { get; private set; }
    public float Defense { get; private set; }
    public float Hp { get; private set; }










    //private readonly CompanionModel _companionModel;
    //private readonly CurrencyModel _currencyModel;
    //private readonly DataTableManager _dataTable;
    //private readonly CompanionService _companionService;
    //private readonly List<CompanionInventoryItem> _items = new();

    //public IReadOnlyList<CompanionInventoryItem> Items => _items;
    //public string SelectedCompanionId { get; private set; }
    //public string Name { get; private set; }
    //public int Level { get; private set; }
    //public float Atk { get; private set; }
    //public float Def { get; private set; }
    //public float Hp { get; private set; }
    //public long LevelUpCost { get; private set; }
    //public bool IsMaxLevel { get; private set; }
    //public bool CanLevelUp { get; private set; }

    //public event Action Changed;

    //public CompanionInventoryViewModel(
    //    CompanionModel companionModel,
    //    CurrencyModel currencyModel,
    //    DataTableManager dataTable,
    //    CompanionService companionService)
    //{
    //    _companionModel = companionModel;
    //    _currencyModel = currencyModel;
    //    _dataTable = dataTable;
    //    _companionService = companionService;

    //    _companionModel.CompanionChanged += OnCompanionChanged;
    //    _currencyModel.PropertyChanged += OnCurrencyChanged;

    //    RefreshItems();

    //    if (_items.Count > 0)
    //    {
    //        SelectCompanion(_items[0].CompanionId);
    //    }
    //}

    //public bool SelectCompanion(string companionId)
    //{
    //    if (_companionModel.GetCompanion(companionId) == null)
    //    {
    //        return false;
    //    }

    //    SelectedCompanionId = companionId;
    //    RefreshSelectedCompanion();
    //    Changed?.Invoke();

    //    return true;
    //}

    //public CompanionLevelUpResult LevelUpSelectedCompanion()
    //{
    //    CompanionLevelUpResult result = _companionService.TryLevelUp(
    //        SelectedCompanionId);

    //    if (result != CompanionLevelUpResult.Success)
    //    {
    //        RefreshSelectedCompanion();
    //        Changed?.Invoke();
    //    }

    //    return result;
    //}

    //public void Dispose()
    //{
    //    _companionModel.CompanionChanged -= OnCompanionChanged;
    //    _currencyModel.PropertyChanged -= OnCurrencyChanged;
    //}

    //private void OnCompanionChanged(string companionId)
    //{
    //    RefreshItems();

    //    if (SelectedCompanionId == companionId)
    //    {
    //        RefreshSelectedCompanion();
    //    }

    //    Changed?.Invoke();
    //}

    //private void OnCurrencyChanged(
    //    object sender,
    //    PropertyChangedEventArgs eventArgs)
    //{
    //    RefreshSelectedCompanion();
    //    Changed?.Invoke();
    //}

    //private void RefreshItems()
    //{
    //    _items.Clear();

    //    foreach (CompanionState companion in _companionModel.Companions)
    //    {
    //        CompanionData companionData = _dataTable.GetCompanionData(
    //            companion.CompanionId);

    //        if (companionData == null)
    //        {
    //            continue;
    //        }

    //        _items.Add(new CompanionInventoryItem(
    //            companion.CompanionId,
    //            companionData.Name,
    //            companion.Level));
    //    }
    //}

    //private void RefreshSelectedCompanion()
    //{
    //    CompanionState companion = _companionModel.GetCompanion(
    //        SelectedCompanionId);

    //    if (companion == null)
    //    {
    //        return;
    //    }

    //    CompanionData companionData = _dataTable.GetCompanionData(
    //        companion.CompanionId);
    //    CompanionLevelData levelData = _dataTable.GetCompanionLevelData(
    //        companion.CompanionId,
    //        companion.Level);
    //    CompanionLevelData nextLevelData = _dataTable.GetCompanionLevelData(
    //        companion.CompanionId,
    //        companion.Level + 1);

    //    if (companionData == null || levelData == null)
    //    {
    //        return;
    //    }

    //    Name = companionData.Name;
    //    Level = companion.Level;
    //    Atk = levelData.BaseAttack;
    //    Def = levelData.BaseDefense;
    //    Hp = levelData.HP;
    //    IsMaxLevel = nextLevelData == null;
    //    LevelUpCost = IsMaxLevel ? 0 : (long)nextLevelData.UpgradeCost;
    //    CanLevelUp = !IsMaxLevel && _currencyModel.DreamFragment >= LevelUpCost;
    //}
}
