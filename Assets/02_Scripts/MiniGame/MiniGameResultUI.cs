using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

public class MiniGameResultUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private TextMeshProUGUI _completionText;
    [SerializeField] private UIButtonComponent _closeButton;

    [Header("연출")]
    [SerializeField] private float _openDuration = 0.25f;
    [SerializeField] private float _closeDuration = 0.2f;

    private UniTaskCompletionSource _closeRequestedSource;

    public override Tween PlayOpenAnimation()
    {
        transform.localScale = Vector3.zero;

        transform.DOKill();
        return transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public override Tween PlayCloseAnimation()
    {
        transform.DOKill();
        return transform.DOScale(0f, _closeDuration).SetEase(Ease.InBack).SetUpdate(true);
    }

    public override void OnClickCloseButton()
    {
        if (null == _closeRequestedSource)
        {
            return;
        }

        _closeRequestedSource.TrySetResult();
    }

    public void SetResult(MiniGameResult result)
    {
        if (null != _gradeText)
        {
            _gradeText.text = result.Grade.ToString();
        }

        if (null != _completionText)
        {
            _completionText.text = $"{result.Accuracy:P0}";
        }
    }

    public async UniTask WaitForCloseAsync(CancellationToken token)
    {
        _closeRequestedSource = new UniTaskCompletionSource();

        if (null != _closeButton)
        {
            _closeButton.BindButtonEvent(OnClickCloseButton);
        }
        else
        {
            Logger.LogError("CloseButton이 연결되지 않아 결과창을 닫을 수 없습니다.");
        }

        try
        {
            await _closeRequestedSource.Task.AttachExternalCancellation(token);
        }
        finally
        {
            if (null != _closeButton)
            {
                _closeButton.UnBindAllButtonEvent();
            }

            _closeRequestedSource = null;
            CloseUI();
        }
    }
}
