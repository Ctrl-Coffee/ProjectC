
using UnityEngine;

public class EnemyBattleUnitModel : BattleUnitModelBase
{
    protected override void UseSkill(int battlePosition, string skillId)
    {
        Debug.Log($"적 {battlePosition} : {skillId} 사용");
        //SkillExecutionData skillExecutionData = new SkillExecutionData(_attack, _criticalChance, _criticalDamageMultiplier);
        //BattleManager.Instance.RequestEnemyUseSkill(battlePosition, skillId, skillExecutionData);
    }
}
