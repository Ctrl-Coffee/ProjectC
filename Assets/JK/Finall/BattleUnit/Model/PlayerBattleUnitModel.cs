public class PlayerBattleUnitModel : BattleUnitModelBase
{
    public PlayerBattleUnitModel(int battlePosition) : base(battlePosition) { }

    public override void SetActive(bool isActive)
    {
        BattleManager.Instance.RequestUpdatePlayerUnitActive(BattlePosition, isActive);
    }

    protected override void UseSkill(int battlePosition, string skillId)
    {
        SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        BattleManager.Instance.RequestPlayerSkillExecution(battlePosition, skillId, skillExecutionData);
    }
}
