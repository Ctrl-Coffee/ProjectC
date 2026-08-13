using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestGrowthUI : MonoBehaviour
{
    [SerializeField] private string _companionId = "Companion_001";

    [ContextMenu("동료 성장 UI 열기")]
    private void OpenGrowthUI()
    {
        OpenGrowthUIAsync().Forget();
    }
    private async UniTask OpenGrowthUIAsync()
    {
        Debug.Log($"용사 레벨: {GameManager.User.Player.Level}");
        OwnedCompanionData ownedData = GetOrCreateOwnedCompanion(_companionId);

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

    private OwnedCompanionData GetOrCreateOwnedCompanion(string companionId)
    {
        foreach(OwnedCompanionData owned in GameManager.User.Companions)
        {
            if (owned.CompanionId == companionId)
            {
                return owned;
            }
        }

        OwnedCompanionData ownedData = new OwnedCompanionData();
        ownedData.CompanionId = companionId;
        ownedData.Level = 1;
        GameManager.User.Companions.Add(ownedData);
        return ownedData;
    }
}
