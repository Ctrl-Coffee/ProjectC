using TMPro;
using UnityEngine;

public class CompanionDetailUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private UIButtonComponent _levelUpButton;
    [SerializeField] private UIButtonComponent _closeButton;

    private System.Func<LevelUpResult> _onClickLevelUp;

    private CompanionState _companionState;

    public void Init(CompanionState companionState, System.Func<LevelUpResult> onClickLevelUp)
    {
        _companionState = companionState;
        Refresh();

        _onClickLevelUp = onClickLevelUp;

        _levelUpButton.BindButtonEvent(OnClickLevelUp);

        _closeButton.BindButtonEvent(OnClickCloseButton);
    }

    private void OnDisable()
    {
        _levelUpButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
    }

    private void OnClickLevelUp()
    {
        var result = _onClickLevelUp?.Invoke();

        if (result == LevelUpResult.Success)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        _id.text = _companionState.CompanionId;
        _level.text = _companionState.Level.ToString();
    }
}
