using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TestGrowthUI : MonoBehaviour
{
    [SerializeField] private string _companionId = "Companion_001";
    [SerializeField] private string _equipmentId = "Equipment_001";

    [ContextMenu("동료 성장 UI 열기")]
    private void OpenGrowthUI()
    {
        OpenGrowthUIAsync().Forget();
    }
    private async UniTask OpenGrowthUIAsync()
    {
        OwnedCompanionData ownedData = GameManager.Companion.GetOwnedCompanion(_companionId);

        if (ownedData == null)
        {
            Debug.LogError($"보유하지 않은 동료입니다. Id: {_companionId}");
            return;
        }

        CompanionGrowthModel model = new CompanionGrowthModel(ownedData);
        CompanionGrowthViewModel viewModel = new CompanionGrowthViewModel(model);
        GameManager.ViewModel.Register(viewModel);

        CompanionGrowthView view = await GameManager.UI.OpenPopupUI<CompanionGrowthView>();

        if (view == null)
        {
            Debug.LogError("CompanionGrowthView를 열지 못했습니다.");
            return;
        }

        view.BindViewModel(viewModel);
    }

    [ContextMenu("용사 성장 UI 열기")]
    private void OpenPlayerGrowthUI()
    {
        OpenPlayerGrowthUIAsync().Forget();
    }

    private async UniTask OpenPlayerGrowthUIAsync()
    {
        PlayerGrowthModel model = new PlayerGrowthModel(GameManager.User.Player);

        PlayerGrowthViewModel viewModel = new PlayerGrowthViewModel(model);
        GameManager.ViewModel.Register(viewModel);

        PlayerGrowthView view = await GameManager.UI.OpenPopupUI<PlayerGrowthView>();

        if (view == null)
        {
            Debug.LogError("PlayerGrowthView를 열지 못했습니다.");
            return;
        }

        view.BindViewModel(viewModel);
    }

    [ContextMenu("테스트 동료 지급")]
    private void AddTestCompanion()
    {
        bool isAdded = GameManager.Companion.AddCompanion(_companionId);
        Debug.Log(isAdded ? "동료 획득" : "이미 보유 중");
    }

    [ContextMenu("테스트 장비 지급")]
    private void AddTestEquipment()
    {
        bool isAdded = GameManager.Equipment.AddEquipment(_equipmentId);
        Debug.Log(isAdded ? "장비 획득" : "이미 보유장비임");
    }

    [ContextMenu("테스트 장비 장착")]
    private void EquipmentTest()
    {
        bool isEquip = GameManager.Equipment.Equip(_equipmentId);
        Debug.Log(isEquip ? "장비 장착" : "장착할 장비 없음");
        
    }

    [ContextMenu("장착 장비 스탯 로그")]
    private void LogEquippedStat()
    {
        OwnedEquipmentData equipped = GameManager.Equipment.GetEquippedEquipment();

        if (equipped == null)
        {
            Debug.Log("장착한 장비가 없습니다.");
            return;
        }

        CharacterStatData stat = GameManager.Growth.GetEquipmentFinalStat(equipped.EquipmentId, equipped.Level);

        if (stat == null)
        {
            return;
        }

        Debug.Log($"장비: {equipped.EquipmentId} / 레벨: {equipped.Level} / Atk: {stat.FinalAtk} / Def: {stat.FinalDef} / Hp: {stat.FinalHp}");
    }
}



