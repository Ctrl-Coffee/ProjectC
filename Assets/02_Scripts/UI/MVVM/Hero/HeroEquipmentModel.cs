using System;
using System.Collections.Generic;

public class HeroEquipmentModel : ModelBase, ContainerPropertyChanged<HeroEquipmentState>
{
    public event Action<string, ContainerPropertyChangedEvent, HeroEquipmentState> ContainerPropertyChanged;

    public Dictionary<string, HeroEquipmentState> Equipments { get => _equipments; }

    private Dictionary<string, HeroEquipmentState> _equipments = new();

    public HeroEquipmentModel(List<EquipmentDto> equipmentDtoes)
    {
        foreach (EquipmentDto equipmentDto in equipmentDtoes)
        {
            HeroEquipmentState state = new HeroEquipmentState(equipmentDto);

            _equipments.Add(state.HeroEquipmentId, state);
        }

        InitializeOnce();
    }


    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Equipments));
    }


    public void AddHeroEquipment(string heroEquipmentId)
    {
        if (!_equipments.ContainsKey(heroEquipmentId))
        {
            HeroEquipmentState state = new HeroEquipmentState(heroEquipmentId, 1);
            _equipments.Add(heroEquipmentId, state);
            ContainerPropertyChanged?.Invoke(nameof(Equipments), ContainerPropertyChangedEvent.Add, state);
            SaveUtil.RequestSaveEquipment();
        }
    }

    public HeroEquipmentState GetHeroEquipment(string heroEquipmentId)
    {
        if (string.IsNullOrEmpty(heroEquipmentId))
        {
            return null;
        }

        return _equipments.TryGetValue(heroEquipmentId, out HeroEquipmentState state) ? state : null;
    }

    public LevelUpResult TryLevelUp(string heroEquipmentId)
    {
        if (!_equipments.TryGetValue(heroEquipmentId, out HeroEquipmentState state))
        {
            return LevelUpResult.Error;
        }

        var equipmentData = GameManager.DataTable.GetEquipmentData(heroEquipmentId);
        var leveldata = GameManager.DataTable.GetEquipmentLevelData(Utils.GetEquipmentLevelDataId(equipmentData.Grade, state.Level));

        if (leveldata.UpgradeCost == 0)
        {
            return LevelUpResult.MaxLevel;
        }

        if (!GameManager.Session.Currency.TrySpendDreamFragment(leveldata.UpgradeCost))
        {
            return LevelUpResult.NotEnoughCurrency;
        }

        state.LevelUp();
        ContainerPropertyChanged?.Invoke(nameof(Equipments), ContainerPropertyChangedEvent.Update, state);

        SaveUtil.RequestSaveEquipment();

        return LevelUpResult.Success;
    }

    public void RemoveHeroEquipment(string heroEquipmentId)
    {
        if (_equipments.TryGetValue(heroEquipmentId, out HeroEquipmentState state))
        {
            _equipments.Remove(heroEquipmentId);
            ContainerPropertyChanged?.Invoke(nameof(Equipments), ContainerPropertyChangedEvent.Remove, state);
        }
    }

    public int GetLevel(string heroEquipmentId)
    {
        return _equipments.TryGetValue(heroEquipmentId, out HeroEquipmentState state) ? state.Level : 0;
    }
}
