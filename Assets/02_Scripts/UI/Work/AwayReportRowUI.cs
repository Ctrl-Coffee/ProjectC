using DG.Tweening;
using TMPro;
using UnityEngine;

public class AwayReportRowUI : MonoBehaviour
{
    private const float COUNT_UP_DURATION = 0.7f;

    private const float ICON_PUNCH_SCALE = 0.25f;
    private const float ICON_PUNCH_DURATION = 0.35f;
    private const int ICON_PUNCH_VIBRATO = 6;
    private const float ICON_PUNCH_ELASTICITY = 0.6f;

    [SerializeField] private RectTransform _icon;
    [SerializeField] private TextMeshProUGUI _txtLabel;
    [SerializeField] private TextMeshProUGUI _txtValue;

    private long _targetAmount;

    public void SetAmount(long amount)
    {
        if (null == _txtValue)
        {
            Logger.LogError($"{name} 의 값 텍스트가 연결되지 않았습니다.");
            return;
        }

        _txtValue.text = Format(amount);
    }

    public void SetLabel(string label)
    {
        if (null == _txtLabel)
        {
            Logger.LogError($"{name} 의 이름 텍스트가 연결되지 않았습니다.");
            return;
        }

        _txtLabel.text = label;
    }

    public Sequence CreateCountUpSequence(long amount)
    {
        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        AppendCountUp(sequence, amount);
        AppendIconPunch(sequence);

        return sequence;
    }

    private void AppendCountUp(Sequence sequence, long amount)
    {
        if (null == _txtValue)
        {
            return;
        }

        _targetAmount = amount;
        _txtValue.text = Format(0);

        Tween tween = DOVirtual.Float(0f, amount, COUNT_UP_DURATION, OnCountUpdated);

        tween.SetEase(Ease.OutQuad).SetUpdate(true);
        tween.OnComplete(OnCountUpCompleted);

        sequence.Insert(0f, tween);
    }

    private void AppendIconPunch(Sequence sequence)
    {
        if (null == _icon)
        {
            return;
        }

        _icon.localScale = Vector3.one;

        Tween tween = _icon.DOPunchScale(Vector3.one * ICON_PUNCH_SCALE, ICON_PUNCH_DURATION, ICON_PUNCH_VIBRATO, ICON_PUNCH_ELASTICITY);

        tween.SetUpdate(true);

        sequence.Insert(0f, tween);
    }

    private void OnCountUpdated(float value)
    {
        _txtValue.text = Format((long)value);
    }

    // 캐스팅 때문에 끝값이 1 모자라게 끝나는 걸 막는다.
    private void OnCountUpCompleted()
    {
        _txtValue.text = Format(_targetAmount);
    }

    private string Format(long amount)
    {
        return $"+{amount:N0}";
    }
}
