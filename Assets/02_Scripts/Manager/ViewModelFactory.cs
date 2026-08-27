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

    public PlayerStatViewModel CreatePlayerStatViewModel()
    {
        PlayerStatModel model = _session.PlayerStat;
        PlayerStatViewModel viewModel = new PlayerStatViewModel(model);
        return viewModel;
    }

    public PerkStatViewModel CreatePerkStatViewModel()
    {
        PerkStatViewModel viewModel = new PerkStatViewModel();
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

    public GachaViewModel CreateGachaViewModel()
    {
        GachaModel model = _session.Gacha;
        GachaViewModel viewModel = new GachaViewModel(model);
        return viewModel;
    }

    public HeroInventoryViewModel CreateHeroInventoryViewModel()
    {
        HeroEquipmentModel model = _session.HeroEquipment;
        HeroInventoryViewModel viewModel = new HeroInventoryViewModel(model);
        return viewModel;
    }

    public HeroEquipmentSlotViewModel CreateHeroEquipmentSlotViewModel(string heroEquipmentId)
    {
        HeroEquipmentModel model = _session.HeroEquipment;
        HeroEquipmentSlotViewModel viewModel = new HeroEquipmentSlotViewModel(model, heroEquipmentId);
        return viewModel;
    }

    public CurrencyViewModel CreateCurrencyViewModel()
    {
        CurrencyModel model = _session.Currency;
        CurrencyViewModel viewModel = new CurrencyViewModel(model);
        return viewModel;
    }
}
