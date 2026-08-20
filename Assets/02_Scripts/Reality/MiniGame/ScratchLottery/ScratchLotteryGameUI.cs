using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using TMPro;
using UnityEngine;

public class ScratchLotteryGameUI : MiniGameBase
{
    [Header("배치")]
    [SerializeField] private GameObject _contentRoot;
    [SerializeField] private ScratchInputArea _inputArea;
    [SerializeField] private ScratchCell[] _cells;
    [SerializeField] private TextMeshProUGUI _txtTimer;
    [SerializeField] private Sprite[] _symbolSprites;

    [Header("긁기")]
    [SerializeField] private int _brushRadius = 6;

    [Header("진행")]
    [SerializeField] private float _playDurationSeconds = 30f;

    [Header("보상 배율")]
    [SerializeField] private float[] _symbolRates = { 0.1f, 0.2f, 0.5f, 1f };
    [SerializeField] private float _match4Multiplier = 1.5f;
    [SerializeField] private float _match5Multiplier = 2f;

    private SymbolDrawer _drawer = new();

    public override async UniTask<MiniGameResult> PlayAsync(MiniGameContext context, CancellationToken token)
    {
        if (ValidateReferences() == false)
        {
            return MiniGameResult.Canceled;
        }

        _contentRoot.SetActive(true);

        ScratchModifier modifier = CreateModifier();
        ScratchSymbol[] symbols = _drawer.Draw(modifier);

        SetupCells(symbols);

        _inputArea.OnScratch += OnScratch;

        try
        {
            await RunTimerAsync(token);
        }

        finally
        {
            _inputArea.OnScratch -= OnScratch;
        }

        ScratchLotteryResult result = _drawer.Judge(symbols, CollectRevealed());

        return ToMiniGameResult(result);
    }

    private void SetupCells(ScratchSymbol[] symbols)
    {
        for (int index = 0; index < _cells.Length; index++)
        {
            _cells[index].Initialize(GetSymbolSprite(symbols[index]));
        }
    }
    
    private Sprite GetSymbolSprite(ScratchSymbol symbol)
    {
        int spriteIndex = (int)symbol - 1;

        if (spriteIndex < 0 || _symbolSprites.Length <= spriteIndex)
        {
            Debug.LogError($"심볼 스프라이트를 찾을수 없습니다. symbol : {symbol}");
            return null;
        }

        return _symbolSprites[spriteIndex];
    }

    private async UniTask RunTimerAsync(CancellationToken token)
    {
        float remainSeconds = _playDurationSeconds;

        while (0f < remainSeconds)
        {
            if (IsAllRevealed())
            {
                break;
            }

            remainSeconds -= GetDeltaTime();

            _txtTimer.text = Mathf.CeilToInt(Mathf.Max(0f, remainSeconds)).ToString();

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }

        _txtTimer.text = "0";
    }

    private bool IsAllRevealed()
    {
        foreach (ScratchCell cell in _cells)
        {
            if (cell.IsRevealed ==  false)
            {
                return false;
            }
        }

        return true;
    }

    private bool[] CollectRevealed()
    {
        bool[] revealed = new bool[_cells.Length];

        for (int index = 0; index < _cells.Length; index++)
        {
            revealed[index] = _cells[index].IsRevealed;
        }

        return revealed;
    }
    private MiniGameResult ToMiniGameResult(ScratchLotteryResult lotteryResult)
    {
        if (lotteryResult.IsSuccess == false)
        {
            return MiniGameResult.Completed(0f);
        }

        MiniGameResult result = MiniGameResult.Completed(GetSymbolRate(lotteryResult.MatchedSymbol));

        result.RewardMultiplier = GetCountMultiplier(lotteryResult.MatchedCount);
        result.Grade = GetGrade(lotteryResult.MatchedSymbol);

        return result;
    }

    private float GetSymbolRate(ScratchSymbol symbol)
    {
        int rateIndex = (int)symbol - 1;

        if (rateIndex < 0 || _symbolRates.Length <= rateIndex)
        {
            Debug.LogError($"심볼 배율을 찾을 수 없습니다. symbol: {symbol}");
            return 0f;
        }

        return _symbolRates[rateIndex];
    }

    private float GetCountMultiplier(int matchedCount)
    {
        if (5 <= matchedCount)
        {
            return _match5Multiplier;
        }

        if (4 == matchedCount)
        {
            return _match4Multiplier;
        }

        return 1f;
    }

    private MiniGameGrade GetGrade(ScratchSymbol symbol)
    {
        switch (symbol)
        {
            case ScratchSymbol.Money: return MiniGameGrade.Perfect;
            case ScratchSymbol.Bed: return MiniGameGrade.Good;
            case ScratchSymbol.Coffee: return MiniGameGrade.Normal;
            case ScratchSymbol.Computer: return MiniGameGrade.Bad;
            default: return MiniGameGrade.Miss;
        }
    }
    private void OnScratch(Vector2 screenPosition)
    {
        foreach (ScratchCell cell in _cells)
        {
            if (cell.IsRevealed)
            {
                continue;
            }

            RectTransform coverRect = cell.CoverRect;

            if (RectTransformUtility.RectangleContainsScreenPoint(coverRect, screenPosition, null) == false)
            {
                continue;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(coverRect, screenPosition, null, out Vector2 localPoint) == false)
            {
                continue;
            }

            Rect rect = coverRect.rect;

            Vector2 normalizedPosition = new Vector2((localPoint.x - rect.xMin) / rect.width, (localPoint.y - rect.yMin) / rect.height);
            
            cell.Erase(normalizedPosition, _brushRadius);
            return;
        }
    }
    // TODO 희준 : WorkStatType에 심볼 확률 항목이 추가되면 퍽에서 읽어오기
    private ScratchModifier CreateModifier()
    {
        return new ScratchModifier();
    }

    private bool ValidateReferences()
    {
        if (null == _contentRoot)
        {
            Debug.LogError("컨텐츠 루트가 연결되지 않았습니다.");
            return false;
        }

        if (null == _inputArea)
        {
            Debug.LogError("긁기 입력 영역이 연결되지 않았습니다.");
            return false;
        }

        if (null == _txtTimer)
        {
            Debug.LogError("타이머 텍스트가 연결되지 않았습니다.");
            return false;
        }

        if (null == _cells || _cells.Length != SymbolDrawer.CELL_COUNT)
        {
            Debug.LogError($"칸이 {SymbolDrawer.CELL_COUNT}개여야 합니다.");
            return false;
        }

        if (null == _symbolSprites || _symbolSprites.Length != _symbolRates.Length)
        {
            Debug.LogError("심볼 스프라이트 개수가 배율 개수와 다릅니다.");
            return false;
        }

        return true;
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
