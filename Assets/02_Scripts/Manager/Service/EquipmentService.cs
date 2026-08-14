using UnityEngine;

public class EquipmentService
{
    private EquipmentGrowthViewModel _equipmentGrowthViewModel;

    public EquipmentGrowthViewModel RequestEquipmentGrowthViewModel(string equipmentId)
    {
        if (_equipmentGrowthViewModel == null)
        {
            CreateEquipmentGrowthViewModel(equipmentId);
        }
        return _equipmentGrowthViewModel;
    }

    public void ReleaseEquipmentGrowthViewModel()
    {
        if (_equipmentGrowthViewModel != null)
        {
            _equipmentGrowthViewModel.UnBind();
            _equipmentGrowthViewModel = null;
        }
    }

    private EquipmentGrowthViewModel CreateEquipmentGrowthViewModel(string equipmentId)
    {
        OwnedEquipmentData ownedData = GameManager.Equipment.GetOwnedEquipment(equipmentId);

        if (ownedData == null)
        {
            Debug.LogError($"보유하지 않은 장비입니다. Id: {equipmentId}");
            return null;
        }

        EquipmentGrowthModel model = new EquipmentGrowthModel(ownedData);
        EquipmentGrowthViewModel viewModel = new EquipmentGrowthViewModel(model);
        _equipmentGrowthViewModel = viewModel;

        return viewModel;
    }
}
