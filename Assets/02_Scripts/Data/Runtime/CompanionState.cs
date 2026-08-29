
public class CompanionState : IStatData
{
    public string CompanionId { get; }

    public int Level { get; private set; }
    public float Attack { get; private set; }
    public float Hp { get; private set; }
    public float Defense { get; private set; }
    public float CriticalRate { get; private set; }
    public float NormalSkillHaste { get; private set; }
    public float SpecialSkillHaste { get; private set; }
    public float CombatPower { get; private set; }

    private StatSum _baseStat;

    private float _hpGrowthPerLevel;
    private float _attackGrowthPerLevel;
    private float _defenseGrowthPerLevel;

    public CompanionState(CompanionDto companionDto)
    {
        CompanionId = companionDto.companionId;
        Level = companionDto.level;

        CompanionData companionData = GameManager.DataTable.GetCompanionData(CompanionId);

        if (null == companionData)
        {
            Logger.LogError($"{CompanionId} 동료 데이터 찾을 수 없음");
            return;
        }

        _hpGrowthPerLevel = companionData.HpGrowthPerLevel;
        _attackGrowthPerLevel = companionData.AttackGrowthPerLevel;
        _defenseGrowthPerLevel = companionData.DefenseGrowthPerLevel;

        GetBaseStat(companionData);
        SetCombatPower();
    }

    public CompanionState(string companionId, int level)
    {
        CompanionId = companionId;
        Level = level;
    }


    // 체력, 공격력, 방어력 만 상승
    // 기본 능력치 + 레벨당 성장치 × (현재 레벨 - 1)
    public void LevelUp()
    {
        Level++;

        Attack = _baseStat.Attack + _attackGrowthPerLevel * (Level - 1);
        Hp = _baseStat.Hp + _hpGrowthPerLevel * (Level - 1);
        Defense = _baseStat.Defense + _defenseGrowthPerLevel * (Level - 1);

        SetCombatPower();
    }

    private void GetBaseStat(CompanionData companionData)
    {
       _baseStat = new StatSum();

        _baseStat.Attack = companionData.BaseAttack;
        _baseStat.Hp = companionData.BaseHp;
        _baseStat.Defense = companionData.BaseDefense;
        _baseStat.CriticalRate = companionData.BaseCriticalChance * Const.PERCENT_TO_RATE;
        _baseStat.NormalSkillHaste = companionData.BasicAttackHaste;
        _baseStat.SpecialSkillHaste = companionData.SignatureSkillHaste;
    }

    private void SetCombatPower()
    {
        CombatPower = CombatPowerCalculator.Calculate(this);
    }
}
