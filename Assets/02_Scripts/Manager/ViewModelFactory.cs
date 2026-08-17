public class ViewModelFactory
{
    private GameSession _session;
    private DataTableManager _dataTable;

    public ViewModelFactory(GameSession session, DataTableManager dataTable)
    {
        _session = session;
        _dataTable = dataTable;
    }

    public PlayerGrowthViewModel CreatePlayerGrowthViewModel()
    {
        PlayerGrowthModel model = _session.PlayerGrowth;
        PlayerGrowthViewModel viewModel = new PlayerGrowthViewModel(model);
        return viewModel;
    }

    public CompanionInventoryViewModel CreateCompanionInventoryViewModel()
    {
        CompanionModel model = _session.Companion;
        CompanionInventoryViewModel viewModel = new CompanionInventoryViewModel(model);
        return viewModel;
    }

    public CompanionInventorySlotViewModel CreateCompanionInventorySlotViewModel(string companionId)
    {
        CompanionModel model = _session.Companion;
        CompanionInventorySlotViewModel viewModel = new CompanionInventorySlotViewModel(model, companionId);
        return viewModel;
    }
}
