public struct ScratchLotteryResult
{
    public ScratchSymbol[] Symbols;
    public bool[] Revealed;
    public ScratchSymbol MatchedSymbol;
    public int MatchedCount;
    public bool IsSuccess;
}

public struct ScratchModifier
{
    public float[] SymbolWeights;
}
