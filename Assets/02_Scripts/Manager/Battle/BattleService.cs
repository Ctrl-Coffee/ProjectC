using UnityEngine;

public class BattleService
{
    public int AlivePlayerCount { get; private set; }
    public int AliveEnemyCount { get; private set; }

    public void InitializeCounts(int alivePlayerCount, int aliveEnemyCount)
    {
        AlivePlayerCount = alivePlayerCount;
        AliveEnemyCount = aliveEnemyCount;
    }

    public void DecreasePlayerCount()
    {
        AlivePlayerCount = Mathf.Max(0, AlivePlayerCount - 1);
    }

    public void DecreaseEnemyCount()
    {
        AliveEnemyCount = Mathf.Max(0, AliveEnemyCount - 1);
    }

    public bool IsSkillUsable(string skillId, bool isPlayer)
    {
        SkillData skillData = GameManager.DataTable.GetSkillData(skillId);

        if (skillData == null)
        {
            return false;
        }

        int aliveAllyCount = isPlayer ? AlivePlayerCount : AliveEnemyCount;
        int aliveEnemyCount = isPlayer ? AliveEnemyCount : AlivePlayerCount;

        return HasValidTarget(SkillTargetType.Enemy, aliveAllyCount, aliveEnemyCount);
    }

    public void ApplyAttack(BattleUnitModelBase targetModel, AttackStats attackStats)
    {
        targetModel.ReceiveAttack(attackStats);
    }

    private bool HasValidTarget(SkillTargetType targetType, int aliveAllyCount, int aliveEnemyCount)
    {
        switch (targetType)
        {
            case SkillTargetType.Self:
                return true;
            case SkillTargetType.Friendly:
                return aliveAllyCount > 1;
            case SkillTargetType.Enemy:
                return aliveEnemyCount > 0;
            default:
                return false;
        }
    }
}
