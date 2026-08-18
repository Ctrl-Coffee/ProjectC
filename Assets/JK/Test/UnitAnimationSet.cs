using UnityEngine;

[CreateAssetMenu(fileName = "UnitAnimationSet", menuName = "Scriptable Objects/UnitAnimationSet")]
public class UnitAnimationSet : ScriptableObject
{
    [SerializeField] private AnimationClip _idle;
    [SerializeField] private AnimationClip _basicAttack;
    [SerializeField] private AnimationClip _skill;
    [SerializeField] private AnimationClip _hit;
    [SerializeField] private AnimationClip _dead;

    public AnimationClip Idle { get { return _idle; } }  
    public AnimationClip BasicAttack { get { return _basicAttack; } }
    public AnimationClip Skill { get { return _skill; } }
    public AnimationClip Hit { get { return _hit; } }
    public AnimationClip Dead { get { return _dead; } }
}