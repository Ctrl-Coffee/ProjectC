public class PlayerBattleUnitModel : BattleUnitModelBase
{
    public PlayerBattleUnitModel(int battlePosition, string uid) : base(battlePosition, uid) { }

    public override void SetActive(bool isActive)
    {
        GameManager.Battle.RequestUpdatePlayerUnitActive(BattlePosition, isActive);
    }

    protected override bool CheckSkillUseable(string skillId)
    {
        bool isUseable = GameManager.Battle.CheckPlayerSkillUsable(skillId);
        return isUseable;
    }

    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance);
        GameManager.Battle.RequestPlayerSkillExecution(battlePosition, skillId, skillExecutionData);
    }
}
