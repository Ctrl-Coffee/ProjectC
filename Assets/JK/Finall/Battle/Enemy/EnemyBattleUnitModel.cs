using UnityEngine;

public class EnemyBattleUnitModel : ModelBase
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

        EnemyData enemyData = GameManager.DataTable.GetEnemyData(dataId);

        InitializeStats(enemyData);
        InitializeSkills(enemyData);
    }

    public override void InitializeOnce()
    {
        OnPropertyChanged(nameof(Hp));
        OnPropertyChanged(nameof(Attack));
        OnPropertyChanged(nameof(Defense));
    }

    private void InitializeStats(EnemyData enemyData)
    {
        Hp = enemyData.BaseHP;
        Attack = enemyData.BaseATK;
        Defense = enemyData.BaseDEF;
    }

    private void InitializeSkills(EnemyData enemyData)
    {
        // 스킬 초기화
    }
}