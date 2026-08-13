using System.ComponentModel;
using UnityEngine;

public class CompanionGrowthViewModel : ViewModelBase<CompanionGrowthModel>
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public float Atk { get; private set; }
    public float Def { get; private set; }
    public float Hp { get; private set; }

    public CompanionGrowthViewModel(CompanionGrowthModel model) : base(model)
    {
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
    
    private void Refresh()
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(_model.CompanionId);
        if (companionData == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id: {_model.CompanionId}");
            return;
        }

        CharacterStatData stat = GameManager.Growth.GetFinalStat(_model.CompanionId, _model.Level);
        if (stat == null) return;
        
        Name = companionData.Name;
        Level = _model.Level;
        Atk = stat.FinalAtk;
        Def = stat.FinalDef;
        Hp = stat.FinalHp;
    }

    public void LevelUp()
    {
        bool canLevelUp = GameManager.Growth.CheckCanLevelUp(_model.CompanionId, _model.Level);

        if (canLevelUp == false)
        {
            Debug.Log("이미 최대 레벨입니다");
            return;
        }

        _model.Level += 1;
        
    }
}
