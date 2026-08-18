using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class BaseBattleUnitView : MonoBehaviour
{
    protected BattleUnitAnimationController _battleUnitAnimationController;

    protected int _formationIndex;

    public abstract bool IsDead { get; }

    protected virtual void Awake()
    {
        Animator animator = GetComponent<Animator>();
        _battleUnitAnimationController = new BattleUnitAnimationController(animator);
    }

    public void Initialize(string dataId, int formationIndex)
    {
        InitializeViewModel(dataId);
        InitializeAnimation(dataId);

        _formationIndex = formationIndex;
    }

    protected abstract void InitializeViewModel(string dataId);
    protected abstract void InitializeAnimation(string dataId);
    protected abstract void BaseAttack();
    protected abstract void UseSkill();
    protected abstract void TakeDamage(float damage);
}