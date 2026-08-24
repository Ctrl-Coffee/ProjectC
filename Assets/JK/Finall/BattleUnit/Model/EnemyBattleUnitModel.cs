public class EnemyBattleUnitModel : BattleUnitModelBase
{
    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        BattleManager.Instance.RequestEnemyUseSkill(battlePosition, skillId, skillExecutionData);
    }
}
