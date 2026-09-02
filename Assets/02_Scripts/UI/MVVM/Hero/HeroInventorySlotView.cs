using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HeroInventorySlotView : ViewBase
{
    [SerializeField] private Image _itemIcon;
    [SerializeField] private Image _selectedImage;
    [SerializeField] private Image _gradeImage;
    [SerializeField] private TMPro.TextMeshProUGUI _level;

    private string _heroEquipmentId;

    private EquipmentType _type;

    private HeroEquipmentSlotViewModel _viewModel;

    public void Init(EquipmentType type, string heroEquipmentId, System.Action<string> onClickDetail, string iconPath)
    {
        _type = type;
        _heroEquipmentId = heroEquipmentId;

        BindViewModel();
        Subscribe();

        GetComponent<EquipmentSlotInput>().Init(_heroEquipmentId, OnClickSlot, onClickDetail);

        LoadIcon(iconPath);

        var equipmentData = GameManager.DataTable.GetEquipmentData(_heroEquipmentId);

        ColorUtility.TryParseHtmlString(Const.GradeColor(equipmentData.EquipmentGrade), out Color newColor);
        _gradeImage.color = newColor;

        SetSelected(_heroEquipmentId == _viewModel.GetEquippedId(_type));
        Refresh();
    }

    private void OnDestroy()
    {
        UnSubscribe();

        if (_viewModel != null)
        {
            _viewModel.UnBind();
            _viewModel = null;
        }
    }

    protected override void BindViewModel()
    {
        _viewModel = GameManager.ViewModel.CreateHeroEquipmentSlotViewModel();
        _viewModel.SetType(_type);
    }

    protected override void Subscribe()
    {
        _viewModel.OnContainerChanged_ViewModel += OnContainerChanged;
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
    }

    protected override void UnSubscribe()
    {
        _viewModel.OnContainerChanged_ViewModel -= OnContainerChanged;
        _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        if (_heroEquipmentId != _viewModel.GetEquippedId(_type))
        {
            SetSelected(false);
        }
        else
        {
            SetSelected(true);
        }
    }

    private void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, HeroEquipmentState state)
    {
        if (changedEvent != ContainerPropertyChangedEvent.Update || state.HeroEquipmentId != _heroEquipmentId)
        {
            return;
        }

        Refresh();
    }

    private void Refresh()
    {
        int level = _viewModel.GetLevel(_heroEquipmentId);
        _level.SetText("Lv.{0}", level);
    }

    private void LoadIcon(string iconPath)
    {
        _itemIcon.sprite = GameManager.Resource.GetLoadedAsset<SpriteAtlas>(AddressablePath.Atlas.EquipmentAtlas)
            .GetSprite(_heroEquipmentId);
    }

    private void SetSelected(bool isSelected)
    {
        _selectedImage.gameObject.SetActive(isSelected);
    }

    private void OnClickSlot()
    {
        if (_viewModel.IsEquipped(_type, _heroEquipmentId) == false)
        {
            _viewModel.Equip(_type, _heroEquipmentId);
        }
        else
        {
            _viewModel.UnEquip(_type);
        }
    }
}
