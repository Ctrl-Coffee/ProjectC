

public class HeroEquipmentState : IStatData
{
    public string HeroEquipmentId { get; }

    public int Level { get; private set; }
    public float Attack { get; private set; }
    public float Hp { get; private set; }
    public float Defense { get; private set; }
    public float CriticalChance { get; private set; }
    public float BasicAttackHaste { get; private set; }
    public float SignatureSkillHaste { get; private set; }
    public float CombatPower { get; private set; }

    private StatSum _baseStat;

    private string _grade;

    public HeroEquipmentState(EquipmentDto equipmentDto)
    {
        HeroEquipmentId = equipmentDto.equipmentId;
        Level = equipmentDto.level; 
        
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(HeroEquipmentId);

        if (null == equipmentData)
        {
            Logger.LogError($"{HeroEquipmentId} 장비 데이터 찾을 수 없음");
            return;
        }

        _grade = equipmentData.Grade;

        GetBaseStat(equipmentData);
        Recalculate();
    }

    public HeroEquipmentState(string id, int level)
    {
        HeroEquipmentId = id;
        Level = level;
    }

    public void LevelUp()
    {
        Level++;
        Recalculate();
    }

    private void GetBaseStat(EquipmentData equipmentData)
    {
        _baseStat = new StatSum();

        _baseStat.Attack = equipmentData.BaseAttack;
        _baseStat.Hp = equipmentData.BaseHp;
        _baseStat.Defense = equipmentData.BaseDefense;
        _baseStat.CriticalChance = equipmentData.BaseCriticalChance;
        _baseStat.BasicAttackHaste = equipmentData.BasicAttackHaste;
        _baseStat.SignatureSkillHaste = equipmentData.SignatureSkillHaste;
    }

    // 공격력·체력·방어력 EquipmentLevelData.StatMultiplier 적용
    // 최종 장비 능력치 = EquipmentData 기본 능력치 × StatMultiplier
    private void Recalculate()
    {
        var equipmentLevelData = GameManager.DataTable.GetEquipmentLevelData(Utils.GetEquipmentLevelDataId(_grade, Level));

        Attack = _baseStat.Attack * equipmentLevelData.StatMultiplier;
        Hp = _baseStat.Hp * equipmentLevelData.StatMultiplier;
        Defense = _baseStat.Defense * equipmentLevelData.StatMultiplier;

        SetCombatPower();
    }

    private void SetCombatPower()
    {
        CombatPower = CombatPowerCalculator.Calculate(this);
    }
}
