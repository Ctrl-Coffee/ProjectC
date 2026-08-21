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
        bool isValid = false;

        switch (result.GachaType)
        {
            case GachaType.Companion:
                isValid = SetCompanionInfo(result.Id);
                break;

            case GachaType.Equipment:
                isValid = SetEquipmentInfo(result.Id);
                break;

            default:
                Debug.LogError($"지원하지 않는 가챠 종류입니다. type : {result.GachaType}");
                break;
        }

        if (isValid == false) return;

        _newBadge.SetActive(result.IsDuplicate == false);
        _rewardText.gameObject.SetActive(result.IsDuplicate);

        if (result.IsDuplicate)
        {
            _rewardText.text = $"+{result.DuplicateReward}";
        }
    }
    private bool SetCompanionInfo(string companionId)
    {
        CompanionData companionData = GameManager.DataTable.GetCompanionData(companionId);

        if (companionData == null)
        {
            Debug.LogError($"동료 데이터를 찾을 수 없습니다. Id : {companionId}");
            return false;
        }

        _nameText.text = companionData.Name;
        _gradeText.text = $"★ {companionData.Grade}";

        return true;
    }

    private bool SetEquipmentInfo(string equipmentId)
    {
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);

        if (equipmentData == null)
        {
            Debug.LogError($"장비 데이터를 찾을 수 없습니다. Id : {equipmentId}");
            return false;
        }

        _nameText.text = equipmentData.Name;
        _gradeText.text = equipmentData.EquipmentGrade.ToString();

        return true;
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
