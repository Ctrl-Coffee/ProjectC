using UnityEngine;

[CreateAssetMenu(fileName = "UnitAnimationSet", menuName = "Scriptable Objects/UnitAnimationSet")]
public class UnitAnimationSet : ScriptableObject
{
    [SerializeField] private AnimationClip _idleClip;
    [SerializeField] private AnimationClip _deathClip;
    [SerializeField] private AnimationClip _basicAttackClip;
    [SerializeField] private AnimationClip _signatureClip;

    public AnimationClip IdleClip { get { return _idleClip; } }
    public AnimationClip DeathClip { get { return _deathClip; } }
    public AnimationClip BasicAttackClip { get { return _basicAttackClip; } }
    public AnimationClip SignatureClip { get { return _signatureClip; } }
}