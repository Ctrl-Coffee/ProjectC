using Unity.Behavior;
using UnityEngine;

public class PlayerBattleUnitView : BattleUnitViewBase
{
    private BlackboardVariable<bool> _isAutoMode;

    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager.Battle.AutoModeChanged += HandleAutoModeChanged;
        SetAutoMode(GameManager.Battle.AutoMode);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.Battle.AutoModeChanged -= HandleAutoModeChanged;
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

        if (!autoMode)
        {
            return;
        }

        if (!_battleUnitViewModel.IsSignatureSkillReady)
        {
            return;
        }

        UseSignatureSkill();
    }

    private void SetAutoMode(bool autoMode)
    {
        _isAutoMode.Value = autoMode;
    }
}