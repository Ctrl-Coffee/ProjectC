using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TMP_Text _damageText;

    [SerializeField] private float _riseDistance = 50f;
    [SerializeField] private float _duration = 0.8f;

    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        UnityUtility.ValidateReference(_damageText, nameof(_damageText));
    }

    public void Show(DamageResult damageResult, Vector2 startPosition)
    {
        SetDamageTextStyle(damageResult);

        _rectTransform.DOKill();
        _rectTransform.anchoredPosition = startPosition;

        _rectTransform.DOAnchorPosY(startPosition.y + _riseDistance, _duration).OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        GameManager.UI.HideDamageText(this);
    }

    private void SetDamageTextStyle(DamageResult damageResult)
    {
        string damageText = damageResult.Damage.ToString("N0");

        if (damageResult.IsCritical)
        {
            damageText = $"<size=130%>{damageText}</size>";
            damageText = $"<color={Const.CRITICAL_DAMAGE_COLOR}>{damageText}!!</color>";
        }
        else
        {
            damageText = $"<color={Const.NORMAL_DAMAGE_COLOR}>{damageText}</color>";
        }

        _damageText.text = damageText;
    }
}