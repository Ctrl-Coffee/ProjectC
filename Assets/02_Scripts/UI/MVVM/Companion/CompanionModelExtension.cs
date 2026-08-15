using UnityEngine;

public class CompanionModelExtension
{
    public class CompanionService
    {
        private CompanionModel _companionModel;
        private CurrencyModel _currencyModel;
        private DataTableManager _dataTable;

        public CompanionService(DataTableManager dataTable)
        {
            _dataTable = dataTable;

            _companionModel = GameManager.Session.Companions;
            _currencyModel = GameManager.Session.Currency;
        }

        public LevelUpResult TryLevelUp(string companionId)
        {
            CompanionState companion = _companionModel.GetCompanion(companionId);

            if (companion == null)
            {
                return LevelUpResult.Error;
            }

            CompanionLevelData nextLevelData = _dataTable.GetCompanionLevelData(companionId, companion.Level + 1);

            if (nextLevelData == null)
            {
                return LevelUpResult.MaxLevel;
            }

            if (!_currencyModel.TrySpendDreamFragment((long)nextLevelData.UpgradeCost))
            {
                return LevelUpResult.NotEnoughCurrency;
            }

            _companionModel.LevelUp(companionId);

            return LevelUpResult.Success;
        }
    }
}
