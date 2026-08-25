using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageTextUI : MonoBehaviour
{
    [SerializeField] private float _riseDistance = 100;
    [SerializeField] private float _duration = 0.8f;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TextMeshProUGUI _damageTxt;

    private RectTransform _rectTransform;
    private Vector2 _startPosition;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }
    public void PlayDamage()
    {
        _rectTransform.DOKill();
        _canvasGroup.DOKill();
        _startPosition = _rectTransform.anchoredPosition;
        _canvasGroup.alpha = 1f;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(_rectTransform.DOAnchorPosY(_startPosition.y + _riseDistance, _duration));
        sequence.Join(_canvasGroup.DOFade(0f, _duration));
        sequence.OnComplete(Deactivate);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void SetDamage(long damage)
    {
        _damageTxt.text = damage.ToString();
    }
    
}
