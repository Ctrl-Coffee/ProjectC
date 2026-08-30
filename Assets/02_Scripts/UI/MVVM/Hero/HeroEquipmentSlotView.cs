using UnityEngine;
using UnityEngine.UI;

public class HeroEquipmentSlotView : ViewBase
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMPro.TextMeshProUGUI _level;

    private string _heroEquipmentId;

    private EquipmentType _type;

    private EquipmentSlotInput _input;
    private HeroEquipmentSlotViewModel _viewModel;

    public void Init(EquipmentType type, System.Action<string> onClickDetail)
    {
        _type = type;

        BindViewModel();
        Subscribe();

        if (TryGetComponent<EquipmentSlotInput>(out var input))
        {
            _input = input;
        }

        Refresh();
        GetComponent<EquipmentSlotInput>().Init(_heroEquipmentId, OnClickSlot, onClickDetail);
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
            Refresh();
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
        _heroEquipmentId = _viewModel.GetEquippedId(_type);

        _input.SetEquipmentId(_heroEquipmentId);

        LoadIcon();

        int level = _viewModel.GetLevel(_heroEquipmentId);
        _level.SetText("Lv.{0}", level);
    }

    private void LoadIcon()
    {
        if (_heroEquipmentId == null)
        {
            _icon.sprite = null;
            _icon.gameObject.SetActive(false);

            _level.gameObject.SetActive(false);

            return;
        }

        _icon.sprite = GameManager.Resource.GetLoadedAsset<Sprite>
            (GameManager.DataTable.GetEquipmentData(_heroEquipmentId).IconSpriteAddressableKey);
        _icon.gameObject.SetActive(true);

        _level.gameObject.SetActive(true);

    }

    private void OnClickSlot()
    {
        _viewModel.UnEquip(_type);

        _level.gameObject.SetActive(false);

        _icon.sprite = null;
        _icon.gameObject.SetActive(false);
    }
}

