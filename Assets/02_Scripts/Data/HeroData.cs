using System;
using System.Collections.Generic;

[Serializable]
public class HeroData : BaseData
{
    public string Name;
    public float BaseAtk;
    public float BaseHp;
    public float BaseDef;
    public float GrowthAtk;
    public float GrowthHp;
    public float GrowthDef;
    public List<int> LevelUpCost;
}
