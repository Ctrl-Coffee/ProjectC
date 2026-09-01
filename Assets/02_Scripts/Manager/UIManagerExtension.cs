using System.Collections.Generic;

/// <summary>
/// 선로딩된 UI를 열고 닫는 확장 함수
/// </summary>
public static class UIManagerExtension
{
    public static void OpenRealHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<RealHudView>();
    }

    public static void OpenDreamHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<DreamHudView>();
    }

    public static void CloseRealHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<RealHudView>();
    }

    public static void CloseDreamHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<DreamHudView>();
    }

    public static void OpenConfirmUI(this UIManager uiManager, ConfirmData confirmData, ConfirmButtonAction buttonAction = null)
    {
        ConfirmUI ui = uiManager.OpenPopupUI<ConfirmUI>();
        ui.SetConfirmUI(confirmData, buttonAction);
    }

    public static void OpenSettingUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<SettingUI>();
    }

    public static void OpenLoginUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<LoginUI>();
    }

    public static CompanionInventoryView OpenCompanionInventory(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<CompanionInventoryView>();
    }

    public static HeroInventoryView OpenHeroInventory(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<HeroInventoryView>();
    }

    public static HeroInfoView OpenHeroInfo(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<HeroInfoView>();
    }

    public static PerkStatView OpenPerkStat(this UIManager uiManager)
    {
        return uiManager.OpenOverlayUI<PerkStatView>();
    }

    public static void OpenCompanionDetailPopup(this UIManager uiManager, CompanionState companionState, System.Func<LevelUpResult> onClickLevelUp)
    {
        CompanionDetailUI ui = uiManager.OpenPopupUI<CompanionDetailUI>();
        ui.Init(companionState, onClickLevelUp);
    }

    public static void OpenEquipmentDetailPopup(this UIManager uiManager
        , System.Func<LevelUpResult> onClickLevelUp, EquipmentData data, string equipmentId)
    {
        EquipmentDetailUI ui = uiManager.OpenPopupUI<EquipmentDetailUI>();

        ui.Init(onClickLevelUp, data, equipmentId);
    }

    public static LoadingUI OpenLoading(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<LoadingUI>();
    }

    public static WorkInfoUI OpenWorkInfoUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<WorkInfoUI>();
    }

    public static PerkInfoUI OpenPerkInfoUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<PerkInfoUI>();
    }

    public static void OpenPerkDetailUI(this UIManager uiManager, string perkId)
    {
        PerkDetailUI ui = uiManager.OpenPopupUI<PerkDetailUI>();

        if (null == ui) return;

        ui.SetPerk(perkId);
    }

    public static T OpenMiniGameUI<T>(this UIManager uiManager) where T : MiniGameBase
    {
        return uiManager.OpenPopupUI<T>();
    }

    public static MiniGameResultUI OpenMiniGameResultUI(this UIManager uiManager)
    {
        return uiManager.OpenOverlayUI<MiniGameResultUI>();
    }

    public static GachaView OpenGachaView(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<GachaView>();
    }

    public static void OpenGachaResultUI(this UIManager uiManager, IReadOnlyList<GachaResultData> results)
    {
        GachaResultUI ui = uiManager.OpenPopupUI<GachaResultUI>();
        ui.Init(results);
    }

    public static void OpenAwayReportUI(this UIManager uiManager, AwayReport report)
    {
        AwayReportUI ui = uiManager.OpenPopupUI<AwayReportUI>();

        if (null == ui) return;

        ui.SetReport(report);
    }

    public static void OpenBattlePreparation(this UIManager uiManager)
    {
        uiManager.OpenContentUI<BattlePreparationUI>();
    }

    public static void CloseBattlePreparation(this UIManager uiManager)
    {
        uiManager.CloseContentUI<BattlePreparationUI>();
    }

    public static void OpenBattleHpBarHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<BattleHpBarHud>();
    }

    public static void CloseBattleHpBarHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<BattleHpBarHud>();
    }

    public static void OpenStageClearUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<StageClearUI>();
    }

    public static void CloseStageClearUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageClearUI>();
    }

    public static void OpenStageFailUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<StageFailUI>();
    }

    public static void CloseStageFailUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageFailUI>();
    }

    public static void OpenStageInfo(this UIManager uiManager, string stageId)
    {
        if (!GameManager.Stage.TrySetStage(stageId))
        {
            return;
        }

        uiManager.OpenPopupUI<StageInfoView>();
    }

    public static void CloseStagePopup(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageInfoView>();
    }
}
