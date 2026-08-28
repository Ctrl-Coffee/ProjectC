using System.Collections.Generic;

public class CompanionFormation
{
    private readonly Dictionary<int, string> _companionIdByBattlePosition = new Dictionary<int, string>();

    public CompanionFormation()
    {
        InitializePositions();
    }

    //TODO 나중에 편성 데이터 가져와서 가져와서 초기화
    private void InitializePositions()
    {
        _companionIdByBattlePosition.Add(Const.COMPANION_BATTLE_POSITIONS[0], "Companion_001");
        _companionIdByBattlePosition.Add(Const.COMPANION_BATTLE_POSITIONS[1], "Companion_002");
    }

    public bool SetCompanionToPosition(int battlePosition, string companionId)
    {
        if (!IsValidPosition(battlePosition))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        if (ContainsCompanion(companionId))
        {
            return false;
        }

        _companionIdByBattlePosition[battlePosition] = companionId;

        return true;
    }

    public bool TrySetCompanionToEmptyPosition(string companionId, out int targetPosition)
    {
        targetPosition = Const.INVALID_BATTLE_POSITION;

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        if (ContainsCompanion(companionId))
        {
            return false;
        }

        foreach (int position in Const.COMPANION_BATTLE_POSITIONS)
        {
            if (!string.IsNullOrWhiteSpace(_companionIdByBattlePosition[position]))
            {
                continue;
            }

            _companionIdByBattlePosition[position] = companionId;
            targetPosition = position;

            return true;
        }

        return false;
    }

    public bool RemoveCompanion(int battlePosition)
    {
        if (!IsValidPosition(battlePosition))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_companionIdByBattlePosition[battlePosition]))
        {
            return false;
        }

        _companionIdByBattlePosition[battlePosition] = null;

        return true;
    }

    public bool TryRemoveCompanion(string companionId, out int targetPosition)
    {
        targetPosition = Const.INVALID_BATTLE_POSITION;

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        foreach (int position in Const.COMPANION_BATTLE_POSITIONS)
        {
            if (_companionIdByBattlePosition[position] != companionId)
            {
                continue;
            }

            _companionIdByBattlePosition[position] = null;
            targetPosition = position;

            return true;
        }

        return false;
    }

    public bool SwapCompanions(int firstPosition, int secondPosition)
    {
        if (!IsValidPosition(firstPosition) || !IsValidPosition(secondPosition))
        {
            return false;
        }

        if (firstPosition == secondPosition)
        {
            return false;
        }

        string firstCompanionId = _companionIdByBattlePosition[firstPosition];
        string secondCompanionId = _companionIdByBattlePosition[secondPosition];

        _companionIdByBattlePosition[firstPosition] = secondCompanionId;
        _companionIdByBattlePosition[secondPosition] = firstCompanionId;

        return true;
    }

    public string GetCompanionId(int battlePosition)
    {
        if (!_companionIdByBattlePosition.TryGetValue(battlePosition, out string companionId))
        {
            return null;
        }

        return companionId;
    }

    private bool IsValidPosition(int battlePosition)
    {
        bool isValidPosition = _companionIdByBattlePosition.ContainsKey(battlePosition);

        return isValidPosition;
    }

    private bool ContainsCompanion(string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        foreach (string positionCompanionId in _companionIdByBattlePosition.Values)
        {
            if (positionCompanionId != companionId)
            {
                continue;
            }

            return true;
        }

        return false;
    }
}