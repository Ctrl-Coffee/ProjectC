using System;
using System.Collections.Generic;

public class ViewModelManager
{
    private PlayerService _playerService;
    private CompanionService _companionService;
    private EquipmentService _equipmentService;

    public PlayerGrowthViewModel RequestGrowthViewModel()
    {
        return _playerService.RequestGrowthViewModel();
    }

    public CompanionGrowthViewModel RequestCompanionGrowthViewModel(string companionId)
    {
        return _companionService.RequestCompanionGrowthViewModel(companionId);
    }

    public EquipmentGrowthViewModel RequestEquipmentGrowthViewModel(string equipmentId)
    {
        return _equipmentService.RequestEquipmentGrowthViewModel(equipmentId);
    }

    public void AllRelease()
    {
        _playerService.ReleaseGrowthViewModel();
        _companionService.ReleaseCompanionGrowthViewModel();
        _equipmentService.ReleaseEquipmentGrowthViewModel();
    }
}
