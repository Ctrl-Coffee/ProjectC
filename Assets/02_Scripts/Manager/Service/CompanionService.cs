using UnityEngine;

public class CompanionService
{
    private CompanionGrowthViewModel _growthModel;

    public CompanionGrowthViewModel RequestCompanionGrowthViewModel(string companionId)
    {
        if(_growthModel == null)
        {
            CreateCompanionGrowthViewModel(companionId);
        }

        return _growthModel;
    }

    public void ReleaseCompanionGrowthViewModel()
    {
        if (_growthModel != null)
        {
            _growthModel.UnBind();
            _growthModel = null;
        }
    }

    private CompanionGrowthViewModel CreateCompanionGrowthViewModel(string companionId)
    {
        OwnedCompanionData ownedData = GameManager.Companion.GetOwnedCompanion(companionId);
        if (ownedData == null)
        {
            Debug.LogError($"보유하지 않은 동료입니다. Id: {companionId}");
            return null;
        }

        CompanionGrowthModel model = new CompanionGrowthModel(ownedData);
        CompanionGrowthViewModel viewModel = new CompanionGrowthViewModel(model);
        _growthModel = viewModel;

        return viewModel;
    }
}