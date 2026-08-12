using DG.Tweening;
using UnityEngine;

public class UIBase : MonoBehaviour
{
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
        transform.localScale = Vector3.zero;

        transform.DOKill();
        return transform.DOScale(1f, 1f).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public virtual Tween PlayCloseAnimation()
    {
        transform.DOKill();
        return transform.DOScale(0f, 1f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }

    public virtual void OnClickCloseButton()
    {
        // TODO(김익환): audio
        CloseUI();
    }
}
