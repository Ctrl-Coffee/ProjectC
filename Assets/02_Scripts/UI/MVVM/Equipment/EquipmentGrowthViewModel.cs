using System.ComponentModel;
using UnityEngine;


public class EquipmentGrowthViewModel : ViewModelBase<EquipmentGrowthModel>
{
    public string Name { get; private set; }
    public int Level { get; private set; }
    public float Atk { get; private set; }
    public float Def { get; private set; }
    public float Hp { get; private set; }

    public string SkillName { get; private set; }

    public float SkillEffect {  get; private set; }

    public EquipmentGrowthViewModel(EquipmentGrowthModel model) : base(model) { }

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
        EquipmentData equipment = GameManager.DataTable.GetEquipmentData(_model.EquipmentId);
        if (equipment == null)
        {
            Debug.LogError($"장비 데이터를 찾을수 없습니다 id : {_model.EquipmentId}");
            return;
        }

        CharacterStatData stat = GameManager.Growth.GetEquipmentFinalStat(_model.EquipmentId, _model.Level);
        if (stat == null) return;

        CharacterSkillEffectData skill = GameManager.Growth.GetEquipmentFinalSkill(_model.EquipmentId, _model.Level);
        if (skill == null) return;
        
        Name = equipment.Name;
        Level = _model.Level;
        Atk = stat.FinalAtk;
        Def = stat.FinalDef;
        Hp = stat.FinalHp;
        SkillName = skill.Skill.Name;
        SkillEffect = skill.FinalEffect;
    }

    public void LevelUp()
    {
        //TODO 희준 : 로그 대신 팝업 필요
        if (GameManager.Growth.IsEquipmentMaxLevel(_model.Level) == true)
        {
            Debug.Log("이미 최대 레벨입니다");
            return;
        }

        if (GameManager.Growth.TryPayEquipmentLevelUpCost(_model.Level) == false)
        {
            Debug.Log("꿈의 조각이 부족합니다");
            return;
        }

        _model.Level += 1;
    }
}
