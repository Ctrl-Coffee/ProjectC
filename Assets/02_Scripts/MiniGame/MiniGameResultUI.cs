using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

public class MiniGameResultUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private TextMeshProUGUI _completionText;
    [SerializeField] private UIButtonComponent _confirmButton;

    [Header("연출")]
    [SerializeField] private float _openDuration = 0.25f;

    private UniTaskCompletionSource _confirmSource;

    protected virtual void Awake()
    {
        this.gameObject.SetActive(false);
    }

    public override void PlayOpenAnimation()
    {
        transform.localScale = Vector3.zero;

        transform.DOKill();
        transform.DOScale(1f, _openDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public async UniTask ShowAsync(MiniGameResult result, CancellationToken token)
    {
        Apply(result);

        this.gameObject.SetActive(true);
        this.transform.SetAsLastSibling();

        PlayOpenAnimation();

        _confirmSource = new UniTaskCompletionSource();

        if (null != _confirmButton)
        {
            _confirmButton.BindButtonEvent(OnClickConfirm);
        }
        else
        {
            Logger.LogWarning("ConfirmButton이 없어 결과창을 닫을 수 없습니다.");
        }

        try
        {
            await _confirmSource.Task.AttachExternalCancellation(token);
        }
        finally
        {
            if (null != _confirmButton)
            {
                _confirmButton.UnBindAllButtonEvent();
            }

            _confirmSource = null;

            if (null != this)
            {
                transform.DOKill();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void Apply(MiniGameResult result)
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

    private void OnClickConfirm()
    {
        if (null == _confirmSource)
        {
            return;
        }

        _confirmSource.TrySetResult();
    }
}
