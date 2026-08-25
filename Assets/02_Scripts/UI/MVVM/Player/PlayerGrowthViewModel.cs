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
        PlayerStatModel stat = GameManager.Session.PlayerStat;
        PlayerData playerData = GameManager.DataTable.GetPlayerData(CharacterId.PLAYER_DATA);

        Name = null == playerData ? string.Empty : playerData.Name;
        Level = _model.Level;
        Atk = stat.Attack;
        Def = stat.Defense;
        Hp = stat.Hp;
    }

    public void LevelUp()
    {
        // TODO 희준 : 로그 대신 팝업필요
        if (GameManager.Growth.IsPlayerMaxLevel(_model.Level) == true)
        {
            Debug.Log("이미 최대 레벨입니다");
            return;
        }

        if (GameManager.Growth.TryPayPlayerLevelUpCost(_model.Level) == false)
        {
            Debug.Log("꿈의 조각이 부족합니다");
            return;
        }

        _model.Level += 1;
    }
   
}
