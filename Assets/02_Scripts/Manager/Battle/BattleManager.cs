using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager
{
    private BattleRoot _battleRoot;

    private readonly BattleUnitModels _battleUnitModels = new BattleUnitModels();
    private readonly CompanionFormation _companionFormation = new CompanionFormation();
    private readonly BattleService _battleService = new BattleService();

    public event Action CompanionChanged;
    public event Action<bool> AutoModeChanged;

    public float BattleTime { get; private set; } 

    public bool AutoMode { get; private set; } 

    public IReadOnlyList<PlayerBattleUnitModel> PlayerBattleUnitModels
    {
        get { return _battleUnitModels.PlayerBattleUnitModels; }
    }

    public IReadOnlyList<EnemyBattleUnitModel> EnemyBattleUnitModels
    {
        get { return _battleUnitModels.EnemyBattleUnitModels; }
    }

    public async UniTask Initialize()
    {
        LoadCompanionPartyResponse loadCompanionPartyResponse = await GameManager.Network.LoadCompanionPartyAsync();
        CompanionPartyDto companionPartyDto = loadCompanionPartyResponse.data;

        _companionFormation.InitializePositions(companionPartyDto);
        _battleUnitModels.Initialize(companionPartyDto);

        CreateBattleRoot();
    }

    public void EnterBattle()
    {
        ResetBattleRoot();
        InitializeStage();

        GameManager.UI.OpenBattlePreparationUI();
    }

    public void ExitBattle()
    {
        if (_battleRoot.IsBattleStarted)
        {
            EndBattle();
        }

        _battleRoot.gameObject.SetActive(false);
    }

    public void StartBattle()
    {
        int dpCost = GameManager.Stage.DpCost;

        if (!GameManager.Session.Currency.TrySpendDreamPoint(dpCost))
        {
            Logger.LogError("꿈 포인트 소비에 실패했습니다.");
            return;
        }

        SubscribeUnitModelDeadStateChangedEvent();

        InitializeBattleCounts();

        GameManager.UI.OpenBattleHud();
        GameManager.UI.OpenDamageTextHud();

        _battleRoot.StartBattle();
    }

    public void RestartBattle()
    {
        EndBattle();

        ResetBattleRoot();
        InitializeStage();

        StartBattle();
    }

    public int GetPlayerTotalCombatPower()
    {
        int playerTotalCombatPower = _battleUnitModels.GetPlayerTotalCombatPower();
        return playerTotalCombatPower;
    }

    public int GetEnemyTotalCombatPower()
    {
        int enemyTotalCombatPower = _battleUnitModels.GetEnemyTotalCombatPower();
        return enemyTotalCombatPower;
    }

    public bool RequestCheckPlayerViewIdle(int battlePosition)
    {
        bool isIdle = _battleRoot.CheckPlayerViewIdle(battlePosition);
        return isIdle;
    }

    public void RequestUnitViewUseSignature(int battlePosition)
    {
        _battleRoot.UseSignature(battlePosition);
    }

    public bool RequestSetCompanionToPosition(int battlePosition, string companionId)
    {
        if (!_companionFormation.SetCompanionToPosition(battlePosition, companionId))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(battlePosition, companionId);
        CompanionChanged?.Invoke();
        return true;
    }

    public bool RequestSetCompanionToEmptyPosition(string companionId)
    {
        if (!_companionFormation.TrySetCompanionToEmptyPosition(companionId, out int battlePosition))
        {
            return false;
        }

        _battleUnitModels.SetCompanion(battlePosition, companionId);
        CompanionChanged?.Invoke();
        return true;
    }

    public bool RequestRemoveCompanion(int battlePosition)
    {
        if (!_companionFormation.RemoveCompanion(battlePosition))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(battlePosition);
        CompanionChanged?.Invoke();
        return true;
    }

    public bool RequestRemoveCompanion(string companionId)
    {
        if (!_companionFormation.TryRemoveCompanion(companionId, out int battlePosition))
        {
            return false;
        }

        _battleUnitModels.RemoveCompanion(battlePosition);
        CompanionChanged?.Invoke();
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

    public void RequestPlayerSkillExecution(int battlePosition, string skillId, AttackerStats skillExecutionData)
    {
        BattleUnitModelBase targetModel = _battleUnitModels.FindEnemyTarget(battlePosition);

        ExcuteSkill(targetModel, skillId, skillExecutionData);
    }

    public void RequestEnemySkillExecution(int battlePosition, string skillId, AttackerStats skillExecutionData)
    {
        BattleUnitModelBase targetModel = _battleUnitModels.FindPlayerTarget(battlePosition);

        ExcuteSkill(targetModel, skillId, skillExecutionData);
    }

    public void RequestUpdatePlayerUnitActive(int battlePosition, bool isActive)
    {
        _battleRoot.UpdateUnitActiveState(battlePosition, isActive, true);
    }

    public void RequestUpdateEnemyUnitActive(int battlePosition, bool isActive)
    {
        _battleRoot.UpdateUnitActiveState(battlePosition, isActive, false);
    }

    public void OnUpdate()
    {
        if (_battleRoot == null)
        {
            return;
        }

        if (!_battleRoot.IsBattleStarted)
        {
            return;
        }

        BattleTime += GameManager.Time.GameDeltaTime;
    }

    public void SetAutoMode(bool autoMode)
    {
        AutoMode = autoMode;
        AutoModeChanged?.Invoke(AutoMode);
    }

    private void ResetBattleTime()
    {
        BattleTime = 0;
    }

    private void EndBattle()
    {
        UnsubscribeUnitModelDeadStateChangedEvents();

        ResetBattleTime();

        GameManager.UI.CloseBattleHud();
        GameManager.UI.CloseDamageTextHud();

        _battleRoot.EndBattle();
    }

    private void InitializeStage()
    {
        string spriteAddressableKey = GameManager.Stage.SpriteAddressableKey;
        _battleRoot.SetBackground(spriteAddressableKey);

        _battleRoot.ResetUnitActiveState();
        _battleUnitModels.InitializeStage();
    }

    private void CreateBattleRoot()
    {
        GameObject prefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.Prefab.BATTLE_ROOT);

        GameObject battleRoot = UnityEngine.Object.Instantiate(prefab);

        if (!battleRoot.TryGetComponent(out _battleRoot))
        {
            Logger.LogError($"{nameof(BattleRoot)} 컴포넌트를 찾을 수 없습니다.");
            UnityEngine.Object.Destroy(battleRoot);
            return;
        }

        _battleRoot.InitializeBattleUnits(_battleUnitModels.PlayerBattleUnitModels, _battleUnitModels.EnemyBattleUnitModels);
        _battleRoot.gameObject.SetActive(false);
    }

    private void ResetBattleRoot()
    {
        _battleRoot.gameObject.SetActive(false);
        _battleRoot.gameObject.SetActive(true);
    }

    private void InitializeBattleCounts()
    {
        int alivePlayerCount = _battleUnitModels.GetAlivePlayerCount();
        int aliveEnemyCount = _battleUnitModels.GetAliveEnemyCount();

        _battleService.InitializeCounts(alivePlayerCount, aliveEnemyCount);
    }

    private void SubscribeUnitModelDeadStateChangedEvent()
    {
        SubscribeUnitModelEvents(_battleUnitModels.PlayerBattleUnitModels, HandlePlayerUnitDeadStateChanged);
        SubscribeUnitModelEvents(_battleUnitModels.EnemyBattleUnitModels, HandleEnemyUnitDeadStateChanged);
    }

    private void UnsubscribeUnitModelDeadStateChangedEvents()
    {
        UnsubscribeUnitModelEvents(_battleUnitModels.PlayerBattleUnitModels, HandlePlayerUnitDeadStateChanged);
        UnsubscribeUnitModelEvents(_battleUnitModels.EnemyBattleUnitModels, HandleEnemyUnitDeadStateChanged);
    }

    private void SubscribeUnitModelEvents(IReadOnlyList<BattleUnitModelBase> battleUnitModelBases, Action<bool> deadStateChangedHandler)
    {
        foreach (BattleUnitModelBase battleUnitModelBase in battleUnitModelBases)
        {
            if (!battleUnitModelBase.IsInitialized)
            {
                continue;
            }

            battleUnitModelBase.DeadStateChanged -= deadStateChangedHandler;
            battleUnitModelBase.DeadStateChanged += deadStateChangedHandler;
        }
    }

    private void UnsubscribeUnitModelEvents(IReadOnlyList<BattleUnitModelBase> battleUnitModelBases, Action<bool> deadStateChangedHandler)
    {
        foreach (BattleUnitModelBase battleUnitModelBase in battleUnitModelBases)
        {
            battleUnitModelBase.DeadStateChanged -= deadStateChangedHandler;
        }
    }

    private bool ExcuteSkill(BattleUnitModelBase targetModel, string skillId, AttackerStats attackerStats)
    {
        if (targetModel == null)
        {
            return false;
        }

        SkillData skillData = GameManager.DataTable.GetSkillData(skillId);

        float damage = attackerStats.Attack * skillData.DamageMultiplier;

        AttackStats attackerStatss = new AttackStats(damage, attackerStats.CriticalChance);

        _battleService.ApplyAttack(targetModel, attackerStatss);
        return true;
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
            EndBattle();
            GameManager.UI.OpenStageFailUI();
        }
        else if (_battleService.AliveEnemyCount <= 0)
        {
            EndBattle();
            AddReward();

            GameManager.UI.OpenBattleVictoryPopup();
        }
    }

    private void AddReward()
    {
        int dreamFragmentReward = GameManager.Stage.DreamShardReward;
        int inspirationReward = GameManager.Stage.InspirationReward;

        GameManager.Session.Currency.AddDreamFragment(dreamFragmentReward);
        GameManager.Session.Currency.AddInspiration(inspirationReward);
    }
}