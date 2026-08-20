using UnityEngine;

public class SymbolDrawer
{
    public const int CELL_COUNT = 5;
    public const int MATCH_COUNT = 3;

    private static readonly float[] DEFAULT_WEIGHT = { 20f, 50f, 20f, 10f};

    public ScratchSymbol[] Draw(ScratchModifier modifier)
    {
        float[] weights = GetWeights(modifier);
        ScratchSymbol[] symbols = new ScratchSymbol[CELL_COUNT];

        for (int index = 0; index < CELL_COUNT; index++)
        {
            symbols[index] = DrawOne(weights);
        }

        return symbols;
    }

    public ScratchLotteryResult Judge(ScratchSymbol[] symbols, bool[] revealed)
    {
        int[] counts = new int[DEFAULT_WEIGHT.Length + 1];

        for (int index = 0; index < symbols.Length; index++)
        {
            if (revealed[index] == false)
            {
                continue;
            }

            counts[(int)symbols[index]]++;
        }

        ScratchSymbol matchedSymbol = ScratchSymbol.None;
        int matchedCount = 0;

        // 5칸이라 MATCH_COUNT 이상인 심볼은 최대 하나뿐이다 (3 + 3 > 5)
        for (int symbolValue = 1; symbolValue < counts.Length; symbolValue++)
        {
            if (counts[symbolValue] < MATCH_COUNT)
            {
                continue;
            }

            matchedSymbol = (ScratchSymbol)symbolValue;
            matchedCount = counts[symbolValue];
            break;
        }

        return new ScratchLotteryResult
        {
            Symbols = symbols,
            Revealed = revealed,
            MatchedSymbol = matchedSymbol,
            MatchedCount = matchedCount,
            IsSuccess = ScratchSymbol.None != matchedSymbol,
        };
    }

    private float[] GetWeights(ScratchModifier modifier)
    {
        if (modifier.SymbolWeights == null || modifier.SymbolWeights.Length != DEFAULT_WEIGHT.Length)
        {
            return DEFAULT_WEIGHT;
        }

        return modifier.SymbolWeights;
    }

    private ScratchSymbol DrawOne(float[] weights)
    {
        float totalWeight = 0f;

        foreach (float weight in weights)
        {
            totalWeight += weight;
        }

        if (totalWeight <= 0f)
        {
            Debug.LogError("심볼 가중치 합이 0이하입니다");
            return ScratchSymbol.None;
        }

        float randomValue = Random.Range(0f, totalWeight);
        float cursor = 0f;

        for (int index = 0; index < weights.Length; index++)
        {
            cursor += weights[index];

            if (randomValue <  cursor)
            {
                return (ScratchSymbol)(index + 1);
            }
        }

        return (ScratchSymbol)weights.Length;
    }
}
