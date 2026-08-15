using DG.Tweening;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    [SerializeField] private bool _isPlayAnimation = true;
    [SerializeField] private RectTransform _panel;
    public bool IsPlayAnimation => _isPlayAnimation;

    protected RectTransform Panel
    {
        get { return _panel; }
    }

    public void OpenUI()
    {
        PlayOpenAnimation();
    }

    public void CloseUI(bool isCloseAll = false)
    {
        GameManager.UI.CloseUI(this, isCloseAll);
    }

    public virtual Tween PlayOpenAnimation()
    {
        if(_isPlayAnimation == false)
            return null;

        _panel.localScale = Vector3.zero;

        _panel.DOKill();
        return _panel.DOScale(1f, 1f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public virtual Tween PlayCloseAnimation()
    {
        _panel.DOKill();
        return _panel.DOScale(0f, 1f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }

    public virtual void OnClickCloseButton()
    {
        // TODO(김익환): audio
        CloseUI();
    }
}
