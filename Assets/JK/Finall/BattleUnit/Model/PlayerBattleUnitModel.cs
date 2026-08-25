using UnityEngine;

public class PlayerBattleUnitModel : BattleUnitModelBase
{
    protected override void UseSkill(int battlePosition, string skillId)
    {
        Debug.Log($"아군 {battlePosition} : {skillId} 사용");
        //SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        //BattleManager.Instance.RequestPlayerUseSkill(battlePosition, skillId, skillExecutionData);
    }
}
