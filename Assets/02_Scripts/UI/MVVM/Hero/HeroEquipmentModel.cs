using System;
using System.Collections.Generic;

public class HeroEquipmentModel : ModelBase, ContainerPropertyChanged<HeroEquipmentState>
{
    public event Action<string, ContainerPropertyChangedEvent, HeroEquipmentState> ContainerPropertyChanged;

    public Dictionary<string, HeroEquipmentState> Equipments { get => _equipments; }

    private Dictionary<string, HeroEquipmentState> _equipments = new();

    public HeroEquipmentModel(IEnumerable<HeroEquipmentState> dbData)
    {
        foreach (HeroEquipmentState data in dbData)
        {
            HeroEquipmentState state = new HeroEquipmentState(data);

            _equipments.Add(state.HeroEquipmentId, state);
        }
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

        var nextleveldata = GameManager.DataTable.GetEquipmentLevelData(state.Level + 1);

        if (nextleveldata == null)
        {
            return LevelUpResult.MaxLevel;
        }

        if (!GameManager.Session.Currency.TrySpendDreamFragment((long)nextleveldata.UpgradeCost))
        {
            return LevelUpResult.NotEnoughCurrency;
        }

        state.LevelUp();
        ContainerPropertyChanged?.Invoke(nameof(Equipments), ContainerPropertyChangedEvent.Update, state);

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
