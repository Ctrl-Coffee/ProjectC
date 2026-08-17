public class CompanionInventorySlotViewModel : ViewModelBase<CompanionModel>
{
    public event System.Action<string, ContainerPropertyChangedEvent, CompanionState> OnContainerChanged_ViewModel;


    private string _companionId;
    public string CompanionId => _companionId;

    public CompanionInventorySlotViewModel(CompanionModel model, string companionId) : base(model)
    {
        _companionId = companionId;
        model.ContainerPropertyChanged += OnContainerChanged;
    }

    public override void UnBind()
    {
        _model.ContainerPropertyChanged -= OnContainerChanged;
        base.UnBind();
    }

    public void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, CompanionState companionState)
    {
        if (propertyName != nameof(CompanionModel.Companions))
        {
            return;
        }

        if (companionState.CompanionId == _companionId)
        {
            OnContainerChanged_ViewModel?.Invoke(propertyName, changedEvent, companionState);
        }
    }

    public int Level => _model.GetCompanion(_companionId).Level;
}
