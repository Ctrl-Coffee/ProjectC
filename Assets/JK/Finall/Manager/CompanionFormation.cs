using System.Collections.Generic;

public class CompanionFormation
{
    private readonly Dictionary<int, string> _companionIdByPosition = new Dictionary<int, string>();

    public CompanionFormation()
    {
        InitializePositions();
    }

    //TODO 나중에 편성 데이터 가져와서 가져와서 초기화
    private void InitializePositions()
    {
        foreach (int position in BattleConstants.COMPANION_POSITIONS)
        {
            _companionIdByPosition.Add(position, null);
        }
    }

    public bool SetCompanionToPosition(int position, string companionId)
    {
        if (!IsValidPosition(position))
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

        _companionIdByPosition[position] = companionId;

        return true;
    }

    public bool TrySetCompanionToEmptyPosition(string companionId, out int targetPosition)
    {
        targetPosition = BattleConstants.INVALID_POSITION;

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        if (ContainsCompanion(companionId))
        {
            return false;
        }

        foreach (int position in BattleConstants.COMPANION_POSITIONS)
        {
            if (!string.IsNullOrWhiteSpace(_companionIdByPosition[position]))
            {
                continue;
            }

            _companionIdByPosition[position] = companionId;
            targetPosition = position;

            return true;
        }

        return false;
    }

    public bool RemoveCompanion(int position)
    {
        if (!IsValidPosition(position))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(_companionIdByPosition[position]))
        {
            return false;
        }

        _companionIdByPosition[position] = null;

        return true;
    }

    public bool TryRemoveCompanion(string companionId, out int targetPosition)
    {
        targetPosition = BattleConstants.INVALID_POSITION;

        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        foreach (int position in BattleConstants.COMPANION_POSITIONS)
        {
            if (_companionIdByPosition[position] != companionId)
            {
                continue;
            }

            _companionIdByPosition[position] = null;
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

        string firstCompanionId = _companionIdByPosition[firstPosition];
        string secondCompanionId = _companionIdByPosition[secondPosition];

        _companionIdByPosition[firstPosition] = secondCompanionId;
        _companionIdByPosition[secondPosition] = firstCompanionId;

        return true;
    }

    public string GetCompanionId(int position)
    {
        if (!_companionIdByPosition.TryGetValue(position, out string companionId))
        {
            return null;
        }

        return companionId;
    }

    private bool IsValidPosition(int position)
    {
        bool isValidPosition = _companionIdByPosition.ContainsKey(position);

        return isValidPosition;
    }

    private bool ContainsCompanion(string companionId)
    {
        if (string.IsNullOrWhiteSpace(companionId))
        {
            return false;
        }

        foreach (string positionCompanionId in _companionIdByPosition.Values)
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