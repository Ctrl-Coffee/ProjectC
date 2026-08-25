using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    private BattleRoot _battleRoot;

    private readonly BattleUnitModels _battleUnitModels = new BattleUnitModels();
    private readonly CompanionFormation _companionFormation = new CompanionFormation();

    private void Awake()
    {
        Instance = this;

        if (_battleRoot == null)
        {
            CreateBattleRootAsync().Forget();
        }
    }


    public void StartBattle()
    {
        //_battleField.InitializeField(mainId, companionIds, enemyIds);
        _battleRoot.StartBattle();
    }

    public void EndBattle()
    {
        //_battleField.InitializeField(mainId, companionIds, enemyIds);
        //_battleField.StartBattle();
    }

    public void RequestInitalizeStage(string stageId)
    {
        _battleUnitModels.InitalizeStage(stageId);
    }

    public bool RequestSetCompanionToPosition(int position, string companionId)
    {
        if (!_companionFormation.SetCompanionToPosition(position, companionId))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(position, companionId);

        return true;
    }

    public bool RequestSetCompanionToEmptyPosition(string companionId)
    {
        if (!_companionFormation.TrySetCompanionToEmptyPosition(companionId, out int position))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(position, companionId);

        return true;
    }

    public bool RequestRemoveCompanion(int position)
    {
        if (!_companionFormation.RemoveCompanion(position))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(position);

        return true;
    }

    public bool RequestRemoveCompanion(string companionId)
    {
        if (!_companionFormation.TryRemoveCompanion(companionId, out int position))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(position);

        return true;
    }

    public bool RequestSwapCompanion(int firstPosition, int secondPosition)
    {
        if (!_companionFormation.SwapCompanions(firstPosition, secondPosition))
        {
            return false;
        }

        string firstPositionCompanionId = _companionFormation.GetCompanionId(firstPosition);
        string secondPositionCompanionId = _companionFormation.GetCompanionId(secondPosition);

        _battleUnitModels.SetCompanion(firstPosition, firstPositionCompanionId);
        _battleUnitModels.SetCompanion(secondPosition, secondPositionCompanionId);

        return true;
    }

    public void RequestPlayerUseSkill(int battlePosition, string skillId, SkillExecutionData skillExecutionData)
    {
        //BaseBattleUnitView target = _battleField.FindEnemyTarget(attackerSlotIndex);

        //ExecuteAttack(target, attackStats);
    }

    public void RequestEnemyUseSkill(int battlePosition, string skillId, SkillExecutionData skillExecutionData)
    {
        //BaseBattleUnitView target = _battleField.FindEnemyTarget(attackerSlotIndex);

        //ExecuteAttack(target, attackStats);
    }

    private async UniTask CreateBattleRootAsync()
    {
        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>("Prefabs/BattleRoot");

        GameObject battleRoot = Instantiate(prefab);

        if (!battleRoot.TryGetComponent(out _battleRoot))
        {
            Debug.LogError($"{nameof(BattleRoot)} 컴포넌트를 찾을 수 없습니다.");
            Destroy(battleRoot);
            return;
        }

        _battleRoot.InitializeBattleUnits(_battleUnitModels.PlayerBattleUnitModels, _battleUnitModels.EnemyBattleUnitModels);
    }
}




    //public void RequestAttackEnemy(int attackerSlotIndex, AttackData attackStats)
    //{
    //    BaseBattleUnitView target = _battleField.FindEnemyTarget(attackerSlotIndex);

    //    ExecuteAttack(target, attackStats);
    //}

    //public void RequestAttackPlayer(int attackerSlotIndex, AttackData attackStats)
    //{
    //    BaseBattleUnitView target = _battleField.FindPlayerTarget(attackerSlotIndex);

    //    ExecuteAttack(target, attackStats);
    //}

    //private void ExecuteAttack(BaseBattleUnitView target, AttackData attackStats)
    //{
    //    if (target == null)
    //    {
    //        return;
    //    }

    //    DefenseStats defenseStats = target.GetDefenseStats();

    //    float damage = BattleUtility.CalculateDamage(attackStats, defenseStats);

    //    target.TakeDamage(damage);
    //}