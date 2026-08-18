using UnityEngine;

public class MainBattleUnitModel : ModelBase
{
    private float _hp;
    private float _attack;
    private float _defense;

    public float Hp
    {
        get { return _hp; }
        set 
        {
            if (_hp == value) { return; }

            _hp = value;
            OnPropertyChanged();
        }
    }

    public float Attack
    {
        get { return _attack; }
        set
        {
            if (_attack == value) { return; }

            _attack = value;
            OnPropertyChanged();
        }
    }

    public float Defense
    {
        get { return _defense; }
        set
        {
            if (_defense == value) { return; }

            _defense = value;
            OnPropertyChanged();
        }
    }

    public void Initialize(string dataId)
    {
        if (string.IsNullOrWhiteSpace(dataId))
        {
            Debug.LogError("");
            return;
        }

        //TODO 주인공 데이터 생기면 수정
        CompanionData companionData = GameManager.DataTable.GetCompanionData(dataId);

        InitializeStats(companionData);
        InitializeSkills(companionData);
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Hp));
        OnPropertyChanged(nameof(Attack));
        OnPropertyChanged(nameof(Defense));
    }

    private void InitializeStats(CompanionData companionData)
    {
        Hp = companionData.BaseHp;
        Attack = companionData.BaseAtk;
        Defense = companionData.BaseDef;
    }

    private void InitializeSkills(CompanionData characterData)
    {
        // 스킬 초기화
    }
}