using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "UseSkillAction", story: "[BattleUnitView] Use [SkillType]", category: "Action", id: "26d57d0d3be28dfbabe44bf4473bc3ef")]
public partial class UseSkillAction : Action
{
    [SerializeReference] public BlackboardVariable<BattleUnitViewBase> BattleUnitView;
    [SerializeReference] public BlackboardVariable<UnitSkillType> SkillType;

    protected override Status OnStart()
    {
        switch (SkillType.Value)
        {
            case UnitSkillType.BasicAttack:
                BattleUnitView.Value.UseBasicAttackSkill();
                break;
            case UnitSkillType.Signature:
                BattleUnitView.Value.UseSignatureSkill();
                break;
            default:
                return Status.Failure;
        }
   
        return Status.Success;
    }
}