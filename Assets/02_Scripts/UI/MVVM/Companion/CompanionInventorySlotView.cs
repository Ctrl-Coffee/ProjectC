using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionInventorySlotView : ViewBase
{
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private TextMeshProUGUI _tempId;
    [SerializeField] private Image _icon;
    [SerializeField] private UIButtonComponent _button;

    private CompanionInventorySlotViewModel _viewModel;

    private event Action<string> _onSelectEvent;
    private string _companionId;

    public void Init(string id, Action<string> action)
    {
        _companionId = id;
        _tempId.text = id;
        _onSelectEvent = action;

        BindViewModel();

        Subscribe(); 
        Refresh();

        // TODO: 테스트 구분 용도
        _icon.color = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
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
        _viewModel = GameManager.ViewModel.CreateCompanionInventorySlotViewModel(_companionId);
    }


    protected override void Subscribe()
    {
        _viewModel.OnContainerChanged_ViewModel += OnContainerChanged;
        _button.BindButtonEvent(OnClickSelectSlot);
    }

    protected override void UnSubscribe()
    {
        _viewModel.OnContainerChanged_ViewModel -= OnContainerChanged;
        _button.UnBindButtonAllEvent();
    }

    private void OnContainerChanged(string propertyName, ContainerPropertyChangedEvent changedEvent, CompanionState companionState)
    {
        if (changedEvent == ContainerPropertyChangedEvent.Update)
        {
            Refresh();
        }
    }

    private void Refresh()
    {
        _level.text = _viewModel.Level.ToString();
    }

    private void OnClickSelectSlot()
    {
        _onSelectEvent?.Invoke(_viewModel.CompanionId);
    }

    protected override void OnPropertyChanged(string propertyName) { }
}
