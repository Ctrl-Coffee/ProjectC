using Unity.Behavior;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider2D))]
public abstract class BattleUnitViewBase : MonoBehaviour
{
    [SerializeField] private BehaviorGraphAgent _behaviorGraphAgent;

    private int _battlePosition;

    private BattleUnitAnimationController _battleUnitAnimationController;
    private BattleUnitViewModel _battleUnitViewModel;

    private BlackboardVariable<SkillReadyEventChannel> _skillReadyEventChannel;

    public int BattlePosition
    {
        get { return _battlePosition; }
    }

    private void Awake()
    {
        Animator animator = GetComponent<Animator>();
        _battleUnitAnimationController = new BattleUnitAnimationController(animator);
        _battleUnitViewModel = new BattleUnitViewModel();

       // _behaviorGraphAgent.GetVariable("SkillReadyEventChannel", out _skillReadyEventChannel);
    }

    private void OnEnable()
    {
        _battleUnitViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnDisable()
    {
        _battleUnitViewModel.PropertyChanged -= OnPropertyChanged;
    }

    private void OnDestroy()
    {
       // _battleUnitViewModel.Dispose();
    }

    public void Initialize(int battlePosition, BattleUnitModelBase baseBattleUnitModel)
    {
        _battlePosition = battlePosition;
        InitializeViewModel(baseBattleUnitModel);

        //InitializeAnimation(dataId);
    }

    public void StartBattle()
    {
        _behaviorGraphAgent.enabled = true;
       // _battleUnitViewModel.StartBattle();
    }

    public void EndBattle()
    {
        _behaviorGraphAgent.enabled = false;
        // _battleUnitViewModel.EndBattle();
    }

    public void UseBasicAttackSkill()
    {
        _battleUnitViewModel.RequestUseBasicAttackSkill(_battlePosition);
        //_battleUnitAnimationController.SetState(UnitState.Idle);
    }

    public void UseActiveSkill()
    {
        _battleUnitViewModel.RequestUseActiveSkill(_battlePosition);
        //_battleUnitAnimationController.SetState(UnitState.Idle);
    }

    private void InitializeViewModel(BattleUnitModelBase baseBattleUnitModel)
    {
        _battleUnitViewModel.Initialize(baseBattleUnitModel);
    }

    //private void InitializeAnimation(string dataId)
    //{
    //    Debug.Log("애니메이션 초기화");
    //    //_battleUnitAnimationController.InitializeAnimation("");
    //}


    private void UpdateHpBar(float hp)
    {
        Debug.Log($"체력 변경 {hp}");
    }

    private void UpdateBasicAttackSkillReady(bool isReady)
    {
        _behaviorGraphAgent.SetVariableValue("IsBasicAttackSkillReady", isReady);

        if (isReady)
        {
            NotifySkillReadyStateChanged();
        }
    }

    private void UpdateActiveSkillReady(bool isReady)
    {
        _behaviorGraphAgent.SetVariableValue("IsActiveSkillReady", isReady);

        if (isReady)
        {
            NotifySkillReadyStateChanged();
        }
    }

    private void UpdateActiveState(bool isDead)
    {
        gameObject.SetActive(!isDead);
    }

    private void NotifySkillReadyStateChanged()
    {
        _skillReadyEventChannel.Value.SendEventMessage();
    }


    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_battleUnitViewModel.Hp):
                UpdateHpBar(_battleUnitViewModel.Hp);
                break;
            case nameof(_battleUnitViewModel.IsBasicAttackSkillReady):
               // UpdateBasicAttackSkillReady(_battleUnitViewModel.IsBasicAttackSkillReady);
                break;
            case nameof(_battleUnitViewModel.IsActiveSkillReady):
              //  UpdateActiveSkillReady(_battleUnitViewModel.IsActiveSkillReady);
                break;
            case nameof(_battleUnitViewModel.IsDead):
                UpdateActiveState(_battleUnitViewModel.IsDead);
                break;
        }
    }
}