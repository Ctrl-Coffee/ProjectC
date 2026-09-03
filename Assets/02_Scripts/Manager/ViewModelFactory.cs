public class ViewModelFactory
{
    private GameSession _session;
    private DataTableManager _dataTable;

    public ViewModelFactory(GameSession session, DataTableManager dataTable)
    {
        _session = session;
        _dataTable = dataTable;
    }

    public HeroInfoViewModel CreateHeroInfoViewModel()
    {
        HeroInfoModel model = _session.HeroInfo;
        HeroInfoViewModel viewModel = new HeroInfoViewModel(model);
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
        HeroInventoryViewModel viewModel = new HeroInventoryViewModel(_session.HeroEquipment, _session.HeroEquiped);
        return viewModel;
    }

    public HeroEquipmentSlotViewModel CreateHeroEquipmentSlotViewModel()
    {
        HeroEquipmentModel model = _session.HeroEquipment;
        HeroEquipmentSlotViewModel viewModel = new HeroEquipmentSlotViewModel(model);
        return viewModel;
    }

    public CurrencyViewModel CreateCurrencyViewModel()
    {
        CurrencyModel model = _session.Currency;
        CurrencyViewModel viewModel = new CurrencyViewModel(model);
        return viewModel;
    }

    public CoffeePotViewModel CreateCoffeePotViewModel()
    {
        CoffeePotModel model = _session.CoffeePot;
        CoffeePotViewModel viewModel = new CoffeePotViewModel(model);
        return viewModel;
    }

    public DreamHudViewModel CreateDreamHudViewModel()
    {
        return new DreamHudViewModel();
    }
}
