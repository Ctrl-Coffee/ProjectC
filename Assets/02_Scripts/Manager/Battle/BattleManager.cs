using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager: UnityEngine.Object
{
    private BattleRoot _battleRoot;

    private readonly BattleUnitModels _battleUnitModels = new BattleUnitModels();
    private readonly CompanionFormation _companionFormation = new CompanionFormation();
    private readonly BattleService _battleService = new BattleService();

    private StageModel _stageModel = new StageModel();

    public StageModel StageModel { get { return _stageModel; } }

    public IReadOnlyList<PlayerBattleUnitModel> PlayerBattleUnitModels
    {
        get { return _battleUnitModels.PlayerBattleUnitModels; }
    }

    public IReadOnlyList<EnemyBattleUnitModel> EnemyBattleUnitModels
    {
        get { return _battleUnitModels.EnemyBattleUnitModels; }
    }

    public void Initalize()
    {
        CreateBattleRoot();
        //_companionFormation.InitializePositions();
        //_battleUnitModels.Initalize();
    }

    public void CreateBattleRoot()
    {
        GameObject prefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.BATTLE_ROOT);

        GameObject battleRoot = Instantiate(prefab);

        if (!battleRoot.TryGetComponent(out _battleRoot))
        {
            Debug.LogError($"{nameof(BattleRoot)} 컴포넌트를 찾을 수 없습니다.");
            Destroy(battleRoot);
            return;
        }

        _battleRoot.InitializeBattleUnits(_battleUnitModels.PlayerBattleUnitModels, _battleUnitModels.EnemyBattleUnitModels);
    }

    public void StartBattle()
    {
        SubscribeUnitModelEvents(_battleUnitModels.PlayerBattleUnitModels, HandlePlayerUnitDeadStateChanged);
        SubscribeUnitModelEvents(_battleUnitModels.EnemyBattleUnitModels, HandleEnemyUnitDeadStateChanged);

        int alivePlayerCount = 3;
        int aliveEnemyCount = 3;

        _battleService.InitializeCounts(alivePlayerCount, aliveEnemyCount);

        GameManager.UI.OpenBattleHpBarHud();

        _battleRoot.StartBattle();
    }

    private void EndBattle()
    {
        UnsubscribeUnitModelEvents(_battleUnitModels.PlayerBattleUnitModels, HandlePlayerUnitDeadStateChanged);
        UnsubscribeUnitModelEvents(_battleUnitModels.EnemyBattleUnitModels, HandleEnemyUnitDeadStateChanged);

        GameManager.UI.CloseBattleHpBarHud();

        _battleRoot.EndBattle();
    }

    public void RequestInitalizeStage(string stageId)
    {
        _battleUnitModels.InitalizeStage(stageId);
        _battleRoot.ResetUnitActiveState();
    }

    public void RequestInitalizeCurrentStage()
    {
        _battleUnitModels.InitalizeStage("");
        _battleRoot.ResetUnitActiveState();
    }

    public void RequestInitalizeNextStage()
    {
        _battleUnitModels.InitalizeStage("");
        _battleRoot.ResetUnitActiveState();
    }

    public bool RequestSetCompanionToPosition(int battlePosition, string companionId)
    {
        if (!_companionFormation.SetCompanionToPosition(battlePosition, companionId))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(battlePosition, companionId);

        return true;
    }

    public bool RequestSetCompanionToEmptyPosition(string companionId)
    {
        if (!_companionFormation.TrySetCompanionToEmptyPosition(companionId, out int battlePosition))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(battlePosition, companionId);

        return true;
    }

    public bool RequestRemoveCompanion(int battlePosition)
    {
        if (!_companionFormation.RemoveCompanion(battlePosition))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(battlePosition);

        return true;
    }

    public bool RequestRemoveCompanion(string companionId)
    {
        if (!_companionFormation.TryRemoveCompanion(companionId, out int battlePosition))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(battlePosition);

        return true;
    }

    public bool RequestSwapCompanion(int firstBattlePosition, int secondBattlePosition)
    {
        if (!_companionFormation.SwapCompanions(firstBattlePosition, secondBattlePosition))
        {
            return false;
        }

        string firstPositionCompanionId = _companionFormation.GetCompanionId(firstBattlePosition);
        string secondPositionCompanionId = _companionFormation.GetCompanionId(secondBattlePosition);

        _battleUnitModels.SetCompanion(firstBattlePosition, firstPositionCompanionId);
        _battleUnitModels.SetCompanion(secondBattlePosition, secondPositionCompanionId);

        return true;
    }

    public bool CheckPlayerSkillUsable(string skillId)
    {
        bool isUsable = _battleService.IsSkillUsable(skillId, true);
        return isUsable;
    }

    public bool CheckEnemySkillUsable(string skillId)
    {
        bool isUsable = _battleService.IsSkillUsable(skillId, false);
        return isUsable;
    }

    public void RequestPlayerSkillExecution(int battlePosition, string skillId, SkillExecutionData skillExecutionData)
    {
        BattleUnitModelBase targetModel = _battleUnitModels.FindEnemyTarget(battlePosition);

        if (targetModel == null)
        {
            return;
        }

        //TODO 스킬아이디로 할만한 처리들

        _battleService.ApplyAttack(targetModel, skillExecutionData);
    }

    public void RequestEnemySkillExecution(int battlePosition, string skillId, SkillExecutionData skillExecutionData)
    {
        BattleUnitModelBase targetModel = _battleUnitModels.FindPlayerTarget(battlePosition);
      
        if (targetModel == null)
        {
            return;
        }

        //TODO 스킬아이디로 할만한 처리들

        _battleService.ApplyAttack(targetModel, skillExecutionData);
    }

    public void RequestUpdatePlayerUnitActive(int battlePosition, bool isActive)
    {
        _battleRoot.UpdateUnitActiveState(battlePosition, isActive, true);
    }

    public void RequestUpdateEnemyUnitActive(int battlePosition, bool isActive)
    {
        _battleRoot.UpdateUnitActiveState(battlePosition, isActive, false);
    }

    private void SubscribeUnitModelEvents(IReadOnlyList<BattleUnitModelBase> battleUnitModelBases, Action<bool> deadStateChangedHandler)
    {
        foreach (BattleUnitModelBase battleUnitModelBase in battleUnitModelBases)
        {
            if (battleUnitModelBase.IsInitialized)
            {
                battleUnitModelBase.DeadStateChanged -= deadStateChangedHandler;
                battleUnitModelBase.DeadStateChanged += deadStateChangedHandler;
            }
        }
    }

    private void UnsubscribeUnitModelEvents(IReadOnlyList<BattleUnitModelBase> battleUnitModelBases, Action<bool> deadStateChangedHandler)
    {
        foreach (BattleUnitModelBase battleUnitModelBase in battleUnitModelBases)
        {
            battleUnitModelBase.DeadStateChanged -= deadStateChangedHandler;
        }
    }

    private void HandlePlayerUnitDeadStateChanged(bool isDead)
    {
        if (isDead)
        {
            _battleService.DecreasePlayerCount();
            CheckBattleOverCondition();
        }
    }

    private void HandleEnemyUnitDeadStateChanged(bool isDead)
    {
        if (isDead)
        {
            _battleService.DecreaseEnemyCount();
            CheckBattleOverCondition();
        }
    }

    private void CheckBattleOverCondition()
    {
        if (_battleService.AlivePlayerCount <= 0)
        {
            GameManager.UI.OpenStageFailUI();
            EndBattle();
        }
        else if (_battleService.AliveEnemyCount <= 0)
        {
            GameManager.UI.OpenStageClearUI();
            EndBattle();
        }
    }
}