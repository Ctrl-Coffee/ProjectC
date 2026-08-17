using TMPro;
using UnityEngine;

public class CompanionDetailUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _id;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private UIButtonComponent _levelUpButton;
    [SerializeField] private UIButtonComponent _closeButton;

    public void Init(CompanionState companionState, System.Action onClickLevelUp)
    {
        _id.text = companionState.CompanionId;
        _level.text = companionState.Level.ToString();

        _levelUpButton.BindButtonEvent(onClickLevelUp);
        _levelUpButton.BindButtonEvent(() => CloseUI());

        _closeButton.BindButtonEvent(() => CloseUI());
    }

    private void OnDisable()
    {
        _levelUpButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
    }
}
