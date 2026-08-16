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

    // TODO; 여기서 GameSession으로 부터 Model을 가져올거임.
    // 필요하다면 DataTableManager도 가져올거임.
    // => 그렇게 ViewModel을 생성하여 반환.
}
