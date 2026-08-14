using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestGrowthUI : MonoBehaviour
{
    [SerializeField] private string _companionId = "Companion_001";
    [SerializeField] private int _level = 3;

    [ContextMenu("동료 성장 UI 열기")]
    private void OpenGrowthUI()
    {
        OpenGrowthUIAsync().Forget();
    }
    private async UniTask OpenGrowthUIAsync()
    {
        OwnedCompanionData ownedData = new OwnedCompanionData();
        ownedData.CompanionId = _companionId;
        ownedData.Level = _level;

        CompanionGrowthModel model = new CompanionGrowthModel(ownedData);
        CompanionGrowthViewModel viewModel = new CompanionGrowthViewModel(model);
        GameManager.ViewModel.Register(viewModel);

        CompanionGrowthView view = await GameManager.UI.OpenPopupUI<CompanionGrowthView>();

        if (view == null)
        {
            Debug.LogError("CompanionGrowthView를 열지 못했습니다.");
            return;
        }

        // view.BindViewModel(viewModel);
    }
}
