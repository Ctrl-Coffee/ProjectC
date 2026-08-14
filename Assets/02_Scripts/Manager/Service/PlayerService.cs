using UnityEngine;

public class PlayerService
{
    private PlayerGrowthViewModel _growthviewModel;

    public PlayerGrowthViewModel RequestGrowthViewModel()
    {
        if(_growthviewModel == null)
        {
            CreatePlayerGrowthViewModel();
        }

        return _growthviewModel;
    }

    public void ReleaseGrowthViewModel()
    {
        if (_growthviewModel != null)
        {
            _growthviewModel.UnBind();
            _growthviewModel = null;
        }
    }

    private PlayerGrowthViewModel CreatePlayerGrowthViewModel()
    {
        PlayerGrowthModel model = new PlayerGrowthModel(GameManager.User.Player);
        PlayerGrowthViewModel viewModel = new PlayerGrowthViewModel(model);
        _growthviewModel = viewModel;

        return viewModel;
    }
}
