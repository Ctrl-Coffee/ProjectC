using Cysharp.Threading.Tasks;
using UnityEngine;

public class TestGrowthUI : MonoBehaviour
{
    [SerializeField] private string _companionId = "Companion001";
    [SerializeField] private int _level = 3;

    private void Start()
    {
        OpenGrowthUIAsync().Forget();
    }

    private async UniTask OpenGrowthUIAsync()
    {
        await UniTask.Delay(1000);   // TODO 희준 : GameManager 초기화 완료 대기로 교체

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

        view.BindViewModel(viewModel);
    }
}
