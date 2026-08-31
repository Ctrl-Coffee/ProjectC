using Unity.Behavior;

public class PlayerBattleUnitView : BattleUnitViewBase
{
    private BlackboardVariable<bool> _isAutoMode;

    protected override void OnEnable()
    {
        GameManager.Battle.AutoModeChanged += HandleAutoModeChanged;
        SetAutoMode(GameManager.Battle.AutoMode);

        base.OnEnable();
    }

    protected override void OnDisable()
    {
        GameManager.Battle.AutoModeChanged += HandleAutoModeChanged;

        base.OnDisable();
    }

    protected override void CacheBehaviorVariables()
    {
        base.CacheBehaviorVariables();

        if (!_behaviorGraphAgent.GetVariable(Const.AUTO_MODE, out _isAutoMode))
        {
            Logger.LogError($"'{Const.AUTO_MODE}' 변수를 찾을 수 없습니다.");
        }
    }

    private void HandleAutoModeChanged(bool autoMode)
    {
        SetAutoMode(autoMode);
    }

    private void SetAutoMode(bool autoMode)
    {
        _isAutoMode.Value = autoMode;
    }
}