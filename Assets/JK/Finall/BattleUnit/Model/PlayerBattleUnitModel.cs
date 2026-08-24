public class PlayerBattleUnitModel : BattleUnitModelBase
{
    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        BattleManager.Instance.RequestPlayerUseSkill(battlePosition, skillId, skillExecutionData);
    }
}
