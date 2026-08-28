using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "WaitUntilBattleUnitIdle", story: "Wait Until [BattleUnitView] Is Idle", category: "Flow", id: "029a8f27dad5213a500e0dfc78761491")]
public partial class WaitUntilBattleUnitIdleAction : Action
{
    [SerializeReference] public BlackboardVariable<BattleUnitViewBase> BattleUnitView;
    protected override Status OnUpdate()
    {
        if (BattleUnitView.Value.IsIdle)
        {
            return Status.Success;
        }

        return Status.Running;
    }
}

