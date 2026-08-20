using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiceGambleGameUI : MiniGameBase
{
    [Header("배치")]
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private TextMeshProUGUI _txtTarget;
    [SerializeField] private Image _imgDice;
    [SerializeField] private Sprite[] _faceSprites;
    [SerializeField] private Sprite[] _rollingSprites;
    [SerializeField] private Button _btnRoll;

    [Header("굴림 연출")]
    [SerializeField] private float _frameIntervalStart = 0.03f;
    [SerializeField] private float _frameIntervalEnd = 0.2f;

    [Header("굴림 연출 - 궤도")]
    [SerializeField] private float _orbitRadius = 500f;
    [SerializeField] private float _orbitTurns = 5f;
    [SerializeField] private float _orbitDuration = 3f;

    [Header("도장 연출")]
    [SerializeField] private TextMeshProUGUI _txtStamp;
    [SerializeField] private float _stampStartScale = 3f;
    [SerializeField] private float _stampDuration = 0.25f;
    [SerializeField] private float _stampAngle = -12f;
    [SerializeField] private Color _successColor = new Color(0.2f, 0.7f, 0.3f);
    [SerializeField] private Color _failColor = new Color(0.8f, 0.2f, 0.2f);

    [Header("결과 대기")]
    [SerializeField] private int _resultDelayMs = 800;

    [Header("보상 배율")]
    [SerializeField] private float _successRate = 1f;
    [SerializeField] private float _failRate = 0.5f;
    [SerializeField] private float _criticalFailRate = 0f;
    [SerializeField] private float _criticalSuccessMultiplier = 2f;

    private Vector2 _diceRestPosition;

    private DiceRoller _roller = new();
    private UniTaskCompletionSource _rollRequestedSource;

    protected override void Awake()
    {
        base.Awake();

        if (null != _imgDice)
        {
            _diceRestPosition = _imgDice.rectTransform.anchoredPosition;
        }
    }
    
    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (ValidateReferences() == false)
        {
            return MiniGameResult.Canceled;
        }

        _contentRoot.SetActive(true);
        _txtStamp.enabled = false;

        DiceModifier modifier = new DiceModifier(); // TODO희준 : 퍽 시스템에서 읽어오기

        int targetValue = _roller.CreateTarget();

        _txtTarget.text = $"목표 : {targetValue}";

        await WaitForRollAsync(token);

        DiceGambleResult diceResult = _roller.Roll(targetValue, modifier);

        await PlayRollAnimationAsync(diceResult.FinalValue, token);
        await PlayStampAsync(diceResult.IsSuccess, token);
        await UniTask.Delay(_resultDelayMs, ignoreTimeScale: true, cancellationToken: token);

        return ToMiniGameResult(diceResult);
    }

    private async UniTask WaitForRollAsync(CancellationToken token)
    {
        _rollRequestedSource = new UniTaskCompletionSource();

        _btnRoll.onClick.AddListener(OnClickRoll);

        try
        {
            await _rollRequestedSource.Task.AttachExternalCancellation(token);
        }
        finally
        {
            _btnRoll.onClick.RemoveListener(OnClickRoll);
            _rollRequestedSource = null;
        }
    }
    private void OnClickRoll()
    {
        _rollRequestedSource?.TrySetResult();
    }

    private bool ValidateReferences()
    {
        if (null == _contentRoot)
        {
            Logger.LogError("컨텐츠 루트가 연결되지 않았습니다.");
            return false;
        }

        if (null == _txtTarget)
        {
            Logger.LogError("목표치 텍스트가 연결되지 않았습니다.");
            return false;
        }

        if (null == _imgDice)
        {
            Logger.LogError("주사위 이미지가 연결되지 않았습니다.");
            return false;
        }

        if (null == _txtStamp)
        {
            Logger.LogError("도장 텍스트가 연결되지 않았습니다.");
            return false;
        }

        if (null == _faceSprites || _faceSprites.Length != DiceRoller.DICE_SIDES)
        {
            Logger.LogError($"주사위 눈금 스프라이트가 {DiceRoller.DICE_SIDES}개여야 합니다.");
            return false;
        }

        if (null == _btnRoll)
        {
            Logger.LogError("Roll 버튼이 연결되지 않았습니다.");
            return false;
        }

        return true;
    }
    private MiniGameResult ToMiniGameResult(DiceGambleResult diceResult)
    {
        float rate = diceResult.IsSuccess ? _successRate
            : diceResult.IsCriticalFail ? _criticalFailRate
            : _failRate;

        MiniGameResult result = MiniGameResult.Completed(rate);

        result.RewardMultiplier = diceResult.IsCriticalSuccess ? _criticalSuccessMultiplier : 1f;
        result.SkipResultPopup = true;

        return result;
    }

    private async UniTask PlayStampAsync(bool isSuccess, CancellationToken token)
    {
        RectTransform stampRect = _txtStamp.rectTransform;

        _txtStamp.text = isSuccess ? "[SUCCESS]" : "[FAILED]";
        _txtStamp.color = isSuccess ? _successColor : _failColor;
        _txtStamp.enabled = true;

        stampRect.DOKill();
        stampRect.localScale = Vector3.one * _stampStartScale;
        stampRect.localEulerAngles = new Vector3(0f, 0f, _stampAngle);

        await stampRect.DOScale(1f, _stampDuration)
            .SetEase(Ease.InQuad)
            .SetUpdate(true)
            .ToUniTask(cancellationToken: token);
    }
    private async UniTask PlayRollAnimationAsync(int finalValue, CancellationToken token)
    {
        RectTransform diceRect = _imgDice.rectTransform;

        float elapsed = 0f;
        float frameTimer = 0f;
        int frameIndex = 0;

        while (elapsed < _orbitDuration)
        {
            float delta = GetDeltaTime();

            elapsed += delta;
            frameTimer += delta;

            float progress = Mathf.Clamp01(elapsed / _orbitDuration);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);

            float angle = _orbitTurns * 360f * eased * Mathf.Deg2Rad;
            float radius = Mathf.Lerp(_orbitRadius, 0f, eased);

            diceRect.anchoredPosition = _diceRestPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            float frameInterval = Mathf.Lerp(_frameIntervalStart, _frameIntervalEnd, eased);

            if (frameTimer >= frameInterval)
            {
                frameTimer = 0f;
                frameIndex++;

                _imgDice.sprite = _rollingSprites[frameIndex % _rollingSprites.Length];
            }

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        diceRect.anchoredPosition = _diceRestPosition;

        _imgDice.sprite = _faceSprites[finalValue - 1];

        _imgDice.transform.DOKill();
        _imgDice.transform.localScale = Vector3.one * 1.4f;
        _imgDice.transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack).SetUpdate(true);
    }
    protected override void ClearGame()
    {
        if (null == _contentRoot) return;

        _contentRoot.SetActive(false);
    }

    public override Tween PlayCloseAnimation()
    {
        if (null != _contentRoot)
        {
            _contentRoot.SetActive(false);
        }

        return base.PlayCloseAnimation();
    }
}