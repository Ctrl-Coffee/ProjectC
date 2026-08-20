using System;
using Unity.Behavior;
using UnityEngine;
using Unity.Properties;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Behavior/Event Channels/CooldownReadyEventChannel")]
#endif
[Serializable, GeneratePropertyBag]
[EventChannelDescription(name: "CooldownReadyEventChannel", message: "CooldownReadyEventChannel", category: "Events", id: "c34ffddd9aa60ab1dec70a2926b0cdd0")]
public sealed partial class CooldownReadyEventChannel : EventChannel { }