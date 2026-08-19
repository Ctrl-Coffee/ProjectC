using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class GachaViewModel : ViewModelBase<GachaModel>
{
    public GachaType CurrentType { get; private set; }
    public string BannerName { get; private set; }
    public long SingleCost { get; private set; }
    public long MultiCost { get; private set; }
    public int SingleDrawCount { get; private set; }
    public int MultiDrawCount { get; private set; }
    public bool CanDrawSingle { get; private set; }
    public bool CanDrawMulti { get; private set; }

    public GachaViewModel(GachaModel model) : base(model) 
    {
        GameManager.Session.Currency.PropertyChanged += OnPropertyChanged;
    }

    public override void InitializeModel()
    {
        Refresh();
        base.InitializeModel();
    }

    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        Refresh();
        base.OnPropertyChanged(sender, e);
    }

    public override void UnBind()
    {
        GameManager.Session.Currency.PropertyChanged -= OnPropertyChanged;
        base.UnBind();
    }

    private void Refresh()
    {
        CurrentType = _model.CurrentType;
        SingleCost = GameManager.Gacha.GetDrawCost(1);
        MultiCost = GameManager.Gacha.GetDrawCost(GachaSystem.MULTI_DRAW_COUNT);
        SingleDrawCount = 1;
        MultiDrawCount = GachaSystem.MULTI_DRAW_COUNT;
        CanDrawSingle = GameManager.Gacha.CheckCanDraw(1);
        CanDrawMulti = GameManager.Gacha.CheckCanDraw(GachaSystem.MULTI_DRAW_COUNT);

        switch (_model.CurrentType)
        {
            case GachaType.Companion:
                BannerName = "동료 소환";
                break;

            case GachaType.Equipment:
                BannerName = "장비 소환";
                break;

            default:
                Debug.LogError($"존재하지 않는 가챠타입입니다 {_model.CurrentType}");
                break;
        }
    }

    public void SelectType(GachaType type )
    {
        _model.CurrentType = type;
    }

    public IReadOnlyList<GachaResultData> Draw(int count)
    {
        if (GameManager.Gacha.TryPayDrawCost(count) == false)
        {
            Debug.Log("몽상의 스크롤이 부족합니다");
            return null;
        }

        IReadOnlyList<string> drawnIds = GameManager.Gacha.Draw(_model.CurrentType, count);

        if (drawnIds == null) return null;

        List<GachaResultData> results = new List<GachaResultData>();

        foreach (string companionId in drawnIds)
        {
            GachaResultData result = GiveCompanion(companionId);

            if (result != null)
            {
                results.Add(result);
            }
        }

        return results;
    }

    private GachaResultData GiveCompanion(string companionId)
    {
        CompanionModel companionModel = GameManager.Session.Companion;

        if (companionModel.GetCompanion(companionId) == null)
        {
            companionModel.AddCompanion(companionId);
            return new GachaResultData(companionId, false, 0);
        }

        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionId);

        if (companionData == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id : {companionId}");
            return null;
        }

        int reward = GameManager.Gacha.GetDuplicateReward(GachaType.Companion, companionData.Grade);
        GameManager.Session.Currency.AddDreamFragment(reward);

        return new GachaResultData(companionId, true, reward);
    }
}
