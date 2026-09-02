

public class HeroEquipmentSlotViewModel : ViewModelBase<HeroEquipmentModel>
{
    public event System.Action<string, ContainerPropertyChangedEvent, HeroEquipmentState> OnContainerChanged_ViewModel;

    //private string _equipmentId;

    private HeroEquipedModel _equipedModel;
    private EquipmentType _type;

    public HeroEquipmentSlotViewModel(HeroEquipmentModel model) : base(model)
    {
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

    public void Equip(EquipmentType type, string equipmentId)
    {
        //_equipmentId = equipmentId;
        _equipedModel.Equip(type, equipmentId);
    }

    public void UnEquip(EquipmentType type)
    {
        //_equipmentId = null;
        _equipedModel.UnEquip(type);
    }

    public void SetType(EquipmentType type)
    {
        _type = type;
    }

    public string GetEquippedId(EquipmentType type)
    {
        return _equipedModel.GetEquipedId(type);
    }

    public EquipmentGrade GetGrade(string id)
    {
        return GameManager.DataTable.GetEquipmentData(id).EquipmentGrade;
    }

    public bool IsEquipped(EquipmentType type, string checkEquipmentId)
    {
        var equiedmentId = _equipedModel.GetEquipedId(type);
        return checkEquipmentId == equiedmentId;
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
        OnContainerChanged_ViewModel?.Invoke(propertyName, changedEvent, state);
    }
}
