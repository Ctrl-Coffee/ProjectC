public class EnemyBattleUnitModel : BattleUnitModelBase
{
    public EnemyBattleUnitModel(int battlePosition) : base(battlePosition) { }

    public override void SetActive(bool isActive)
    {
        BattleManager.Instance.RequestUpdateEnemyUnitActive(BattlePosition, isActive);
    }

    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        BattleManager.Instance.RequestEnemySkillExecution(battlePosition, skillId, skillExecutionData);
    }
}
