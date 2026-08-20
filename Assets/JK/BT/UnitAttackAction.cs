using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UnitAttackAction", story: "[BattleUnitView] Use [Type]", category: "Action", id: "26d57d0d3be28dfbabe44bf4473bc3ef")]
public partial class UnitAttackAction : Action
{
    [SerializeReference] public BlackboardVariable<BaseBattleUnitView> BattleUnitView;
    [SerializeReference] public BlackboardVariable<AttackType> Type;
    protected override Status OnStart()
    {
        switch (Type.Value)
        {
            case AttackType.BaseAttack:
                BattleUnitView.Value.BaseAttack();
                break;
            case AttackType.Skill:
                BattleUnitView.Value.UseSkill();
                break;
        }
   
        return Status.Success;
    }
}

