using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 완료 시점을 알아야 하나?
/// Yes: Async 함수 - ex) fade, loading, 선택 결과 기다리기, 후속 작업 필요한 ui
/// No: void 함수
/// 예시 함수 참고
/// </summary>
public static class UIManagerExtension
{
    public static void OpenRealHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<RealHudView>().Forget();
    }

    public static void OpenDreamHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<DreamHudView>().Forget();
    }

    public static void CloseRealHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<RealHudView>().Forget();
    }

    public static void CloseDreamHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<DreamHudView>().Forget();
    }

    public static async void OpenConfirmUI(this UIManager uiManager, ConfirmData confirmData, ConfirmButtonAction buttonAction = null)
    {
        var ui = await uiManager.OpenPopupUI<ConfirmUI>();
        ui.SetConfirmUI(confirmData, buttonAction);
    }

    public static void OpenSettingUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<SettingUI>().Forget();
    }

    public static void OpenLoginUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<LoginUI>().Forget();
    }

    public static UniTask<CompanionInventoryView> OpenCompanionInventory(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<CompanionInventoryView>();
    }

    public static UniTask<HeroInventoryView> OpenHeroInventory(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<HeroInventoryView>();
    }

    public static UniTask<HeroInfoView> OpenHeroInfo(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<HeroInfoView>();
    }

    public static UniTask<PerkStatView> OpenPerkStat(this UIManager uiManager)
    {
        return uiManager.OpenOverlayUI<PerkStatView>();
    }

    public static async void OpenCompanionDetailPopup(this UIManager uiManager, CompanionState companionState, System.Func<LevelUpResult> onClickLevelUp)
    {
        var ui = await uiManager.OpenPopupUI<CompanionDetailUI>();
        ui.Init(companionState, onClickLevelUp);
    }

    public static async void OpenEquipmentDetailPopup(this UIManager uiManager
        , System.Func<LevelUpResult> onClickLevelUp, EquipmentData data, string equipmentId)
    {
        var ui = await uiManager.OpenPopupUI<EquipmentDetailUI>();


        //무기 = 공격력, 공격속도, 스킬 쿨다운 감소,
        //의복 = 체력, 방어력,
        //장신구 = 공격력, 치명타율,


        ui.Init(onClickLevelUp, data, equipmentId);
    }

    public static UniTask<LoadingUI> OpenLoading(this UIManager uiManager)
    {
        return uiManager.OpenContentUI<LoadingUI>();
    }

    public static UniTask<WorkInfoUI> OpenWorkInfoUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<WorkInfoUI>();
    }

    public static UniTask<PerkInfoUI> OpenPerkInfoUI(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<PerkInfoUI>();
    }

    public static async void OpenPerkDetailUI(this UIManager uiManager, string perkId)
    {
        var ui = await uiManager.OpenPopupUI<PerkDetailUI>();

        if (null == ui) return;

        ui.SetPerk(perkId);
    }

    public static UniTask<T> OpenMiniGameUI<T>(this UIManager uiManager) where T : MiniGameBase
    {
        return uiManager.OpenPopupUI<T>();
    }

    public static UniTask<MiniGameResultUI> OpenMiniGameResultUI(this UIManager uiManager)
    {
        return uiManager.OpenOverlayUI<MiniGameResultUI>();
    }

    public static UniTask<GachaView> OpenGachaView(this UIManager uiManager)
    {
        return uiManager.OpenPopupUI<GachaView>();
    }

    public static async void OpenGachaResultUI(this UIManager uiManager, IReadOnlyList<GachaResultData> results)
    {
        GachaResultUI ui = await uiManager.OpenPopupUI<GachaResultUI>();
        ui.Init(results);
    }

    public static async void OpenAwayReportUI(this UIManager uiManager, AwayReport report)
    {
        var ui = await uiManager.OpenPopupUI<AwayReportUI>();

        if (null == ui) return;

        ui.SetReport(report);
    }

    public static void OpenBattlePreparation(this UIManager uiManager)
    {
        uiManager.OpenContentUI<BattlePreparationUI>().Forget();
    }

    public static void CloseBattlePreparation(this UIManager uiManager)
    {
        uiManager.CloseContentUI<BattlePreparationUI>().Forget();
    }

    public static void OpenBattleHpBarHud(this UIManager uiManager)
    {
        uiManager.OpenHUDUI<BattleHpBarHud>().Forget();
    }

    public static void CloseBattleHpBarHud(this UIManager uiManager)
    {
        uiManager.CloseHUDUI<BattleHpBarHud>().Forget();
    }

    public static void OpenStageClearUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<StageClearUI>().Forget();
    }

    public static void CloseStageClearUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageClearUI>().Forget();
    }

    public static void OpenStageFailUI(this UIManager uiManager)
    {
        uiManager.OpenPopupUI<StageFailUI>().Forget();
    }

    public static void CloseStageFailUI(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageFailUI>().Forget();
    }

    public static async UniTask OpenStageInfo(this UIManager uiManager, string stageId)
    {
        StageInfoView stageInfoView = await uiManager.OpenPopupUI<StageInfoView>();
        GameManager.Stage.SetStage(stageId);   
    }

    public static void CloseStagePopup(this UIManager uiManager)
    {
        uiManager.ClosePopupUI<StageInfoView>().Forget();
    }
}
