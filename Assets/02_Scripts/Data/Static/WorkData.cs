using System;

[Serializable]
public class WorkData : BaseData
{
    public string Name;
    public string WorkType;
    public string MiniGame;
    public int ReqEnergy;

    public float DurationSeconds;

    public int RewardMoney;
    public int RewardDP;
    public string IconKey;
    public string Description;

    private WorkType _workType;
    private MiniGameType _miniGameType;
    private bool _isParsed = false;

    public WorkType Type
    {
        get
        {
            EnsureParsed();
            return _workType;
        }
    }

    public MiniGameType MiniGameType
    {
        get
        {
            EnsureParsed();
            return _miniGameType;
        }
    }

    private void EnsureParsed()
    {
        if (_isParsed)
        {
            return;
        }

        _isParsed = true;

        _workType = Utils.ParseEnum<WorkType>(WorkType);
        _miniGameType = Utils.ParseEnum<MiniGameType>(MiniGame);
    }
}
