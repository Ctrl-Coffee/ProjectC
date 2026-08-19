using DG.Tweening;
using TMPro;
using UnityEngine;

public class GachaResultSlotUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private GameObject _newBadge;
    [SerializeField] private TextMeshProUGUI _rewardText;

    public const float OpenDuration = 0.3f;
    public const float CloseDuration = 0.2f;

    public void Init(GachaResultData result)
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(result.Id);

        if (companionData == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id : {result.Id}");
            return;
        }

        _nameText.text = companionData.Name;
        _gradeText.text = $"★ {companionData.Grade}";

        _newBadge.SetActive(result.IsDuplicate == false);
        _rewardText.gameObject.SetActive(result.IsDuplicate);

        if (result.IsDuplicate)
        {
            _rewardText.text = $"+{result.DuplicateReward}";
        }
    }
    public override Tween PlayOpenAnimation()
    {
        if (IsPlayAnimation == false) return null;

        _panel.localScale = Vector3.zero;
        _panel.DOKill();

        return _panel.DOScale(1f, OpenDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }
    public override Tween PlayCloseAnimation()
    {
        _panel.DOKill();

        return _panel.DOScale(0f, CloseDuration).SetEase(Ease.InBack).SetUpdate(true);
    }

    public void Hide()
    {
        _panel.localScale = Vector3.zero;
    }
}
