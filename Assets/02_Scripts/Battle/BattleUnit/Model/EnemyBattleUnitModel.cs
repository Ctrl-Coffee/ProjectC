public class EnemyBattleUnitModel : BattleUnitModelBase
{
    public EnemyBattleUnitModel(int battlePosition, string uId) : base(battlePosition, uId) { }

    public override void SetActive(bool isActive)
    {
        GameManager.Battle.RequestUpdateEnemyUnitActive(BattlePosition, isActive);
    }

    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance);
        GameManager.Battle.RequestEnemySkillExecution(battlePosition, skillId, skillExecutionData);
    }
}
