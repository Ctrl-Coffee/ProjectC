using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/SkillReadyEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "SkillReadyEvent", message: "[UnitSkillType]", category: "Events", id: "ebe3acf13ce18166d04ed97264d935d0")]
public sealed partial class SkillReadyEvent : EventChannel<UnitSkillType> 
{
    public override Delegate CreateEventHandlerWithoutNotify(BlackboardVariable[] vars, System.Action callback)
    {
        EventHandlerDelegate del = (value) =>
        {
            BlackboardVariable<UnitSkillType> var0 = vars[0] as BlackboardVariable<UnitSkillType>;
            if (var0 != null) var0.Value = value;
            callback();
        };

        return del;
    }
}

