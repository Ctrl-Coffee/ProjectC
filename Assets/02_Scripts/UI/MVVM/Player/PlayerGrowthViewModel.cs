using System.ComponentModel;
using UnityEngine;

public class PlayerGrowthViewModel : ViewModelBase<PlayerGrowthModel>
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public float Atk { get; private set; }
    public float Def { get; private set; }
    public float Hp { get; private set; }

    public PlayerGrowthViewModel(PlayerGrowthModel model) : base(model)
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
        CharacterStatData stat = GameManager.Growth.GetPlayerFinalStat(_model.Level);
        if (stat == null) return;
        // TODO 희준 : 이름 데이터 정해지면 교체
        Name = "임시용사";
        Level = _model.Level;
        Atk = stat.FinalAtk;
        Def = stat.FinalDef;
        Hp = stat.FinalHp;
    }

    public void LevelUp()
    {
        bool canLevelUp = GameManager.Growth.CheckPlayerCanLevelUp(_model.Level);

        if (canLevelUp == false)
        {
            Debug.Log("이미 최대 레벨입니다");
            return;
        }

        _model.Level += 1;
    }
   
}
