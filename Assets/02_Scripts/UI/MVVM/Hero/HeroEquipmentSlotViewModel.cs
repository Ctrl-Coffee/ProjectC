

public class HeroEquipmentSlotViewModel : ViewModelBase<HeroEquipmentModel>
{
    public event System.Action<string, ContainerPropertyChangedEvent, HeroEquipmentState> OnContainerChanged_ViewModel;

    private string _equipmentId;

    private HeroEquipedModel _equipedModel;

    public HeroEquipmentSlotViewModel(HeroEquipmentModel model, string equipmentId) : base(model)
    {
        _equipmentId = equipmentId;
        _equipedModel = GameManager.Session.HeroEquiped;
        _equipedModel.PropertyChanged += OnPropertyChanged;
        _model.ContainerPropertyChanged += OnContainerChanged;
    }

    public override void UnBind()
    {
        _model.ContainerPropertyChanged -= OnContainerChanged;
        _equipedModel.PropertyChanged -= OnPropertyChanged;
        _equipedModel = null;
        base.UnBind();
    }

    public void Equip(EquipmentType type, string id)
    {
        _equipedModel.Equip(type, id);
    }

    public void UnEquip(EquipmentType type)
    {
        _equipedModel.UnEquip(type);
    }

    public string GetEquippedId(EquipmentType type)
    {
        return _equipedModel.GetEquipedId(type);
    }

    public bool IsEquipped(EquipmentType type)
    {
        var equiedmentId = _equipedModel.GetEquipedId(type);
        if (equiedmentId == null)
        {
            return false;
        }

        return equiedmentId == _equipmentId;
    }

    public int GetLevel(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return 0;
        }

        return _model.GetHeroEquipment(id).Level;
    }

    private void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, HeroEquipmentState state)
    {
        if (propertyName != nameof(HeroEquipmentModel.Equipments))
        {
            return;
        }

        if (state.HeroEquipmentId == _equipmentId)
        {
            OnContainerChanged_ViewModel?.Invoke(propertyName, changedEvent, state);
        }
    }
}
