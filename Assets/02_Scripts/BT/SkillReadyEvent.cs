using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/SkillReadyEvent")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "SkillReadyEvent", message: "[UnitSkillType]", category: "Events", id: "ebe3acf13ce18166d04ed97264d935d0")]
public sealed partial class SkillReadyEvent : EventChannel<UnitSkillType> { }

