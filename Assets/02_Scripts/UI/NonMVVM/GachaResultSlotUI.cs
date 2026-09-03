using DG.Tweening;
using TMPro;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GachaResultSlotUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _gradeText;
    [SerializeField] private GameObject _newBadge;
    [SerializeField] private TextMeshProUGUI _rewardText;
    [SerializeField] private GameObject _imgStar;
    [SerializeField] private Image _iconImage;

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
                Logger.LogError($"지원하지 않는 가챠 종류입니다. type : {result.GachaType}");
                break;
        }

        if (isValid == false) return;

        _imgStar.SetActive(result.GachaType == GachaType.Companion);
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
            Logger.LogError($"동료 데이터를 찾을 수 없습니다. Id : {companionId}");
            return false;
        }

        _nameText.text = companionData.Name;
        _gradeText.text = $" {companionData.Grade}";

        SetIcon(companionData.SlotSpriteAddressableKey, companionId);

        return true;
    }

    private bool SetEquipmentInfo(string equipmentId)
    {
        EquipmentData equipmentData = GameManager.DataTable.GetEquipmentData(equipmentId);

        if (equipmentData == null)
        {
            Logger.LogError($"장비 데이터를 찾을 수 없습니다. Id : {equipmentId}");
            return false;
        }

        _nameText.text = equipmentData.Name;
        _gradeText.text = equipmentData.EquipmentGrade.ToString();

        SetIconFromAtlas(equipmentId);

        return true;
    }
    private void SetIcon(string iconPath, string id)
    {
        if (string.IsNullOrEmpty(iconPath))
        {
            Logger.LogWarning($"아이콘 경로가 비어 있습니다. Id : {id}");

            _iconImage.enabled = false;
            return;
        }

        ApplyIcon(GameManager.Resource.GetLoadedAsset<Sprite>(iconPath), id);
    }
    private void SetIconFromAtlas(string equipmentId)
    {
        SpriteAtlas atlas = GameManager.Resource.GetLoadedAsset<SpriteAtlas>(AddressablePath.Atlas.EquipmentAtlas);

        if (atlas == null)
        {
            Logger.LogWarning($"장비 아틀라스를 찾을수 없습니다. {AddressablePath.Atlas.EquipmentAtlas}");
            _iconImage.enabled = false;
            return;
        }

        ApplyIcon(atlas.GetSprite(equipmentId), equipmentId);
    }

    private void ApplyIcon(Sprite icon, string id)
    {
        if (icon == null)
        {
            Logger.LogWarning($"아이콘을 찾을수 없습니다. Id : {id}");

            _iconImage.enabled = false;
            return;
        }

        _iconImage.sprite = icon;
        _iconImage.enabled = true;
    }
    public override Tween PlayOpenAnimation()
    {
        if (IsPlayAnimation == false) return null;

        _panel.localScale = Vector3.zero;
        _panel.DOKill();
        return _panel.DOScale(1f, OpenDuration).SetEase(Ease.OutBack).SetUpdate(true).OnStart(PlaySlotSFX);
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

    private void PlaySlotSFX()
    {
        GameManager.Sound.PlaySFX(AddressablePath.Audio.GACHA_SLOT);
    }
}
