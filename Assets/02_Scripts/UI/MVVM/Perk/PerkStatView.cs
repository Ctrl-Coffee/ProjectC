using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PerkStatView : ViewBase
{
    [SerializeField] private PerkBuffSlotUI[] _perkSlots;
    [SerializeField] private TextMeshProUGUI _perkEmptyText;
    [SerializeField] private UIButtonComponent _btnClose;

    private PerkStatViewModel _viewModel;

    private void OnEnable()
    {
        if (_viewModel == null)
        {
            BindViewModel();
        }

        Subscribe();

        if (null != _btnClose)
        {
            _btnClose.BindButtonEvent(OnClickClose);
        }
    }

    private void OnDisable()
    {
        if (null != _btnClose)
        {
            _btnClose.UnBindButtonAllEvent();
        }

        UnSubscribe();
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
        _viewModel = GameManager.ViewModel.CreatePerkStatViewModel();
    }

    protected override void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
        _viewModel.SubscribePerkChanged();

        _viewModel.InitializeModel();

        RefreshPerkBuff();
    }

    protected override void UnSubscribe()
    {
        if (_viewModel != null)
        {
            _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
            _viewModel.UnSubscribePerkChanged();
        }
    }

    protected override void OnPropertyChanged(string propertyName)
    {
        RefreshPerkBuff();
    }

    private void OnClickClose()
    {
        CloseUI();
    }

    private void RefreshPerkBuff()
    {
        if (_viewModel == null) return;

        IReadOnlyList<PerkBuffInfo> perkBuffs = _viewModel.PerkBuffs;

        int buffCount = null == perkBuffs ? 0 : perkBuffs.Count;

        if (null != _perkEmptyText)
        {
            _perkEmptyText.text = Const.NO_PERK_BUFF;
            _perkEmptyText.gameObject.SetActive(0 == buffCount);
        }

        if (null == _perkSlots)
        {
            return;
        }

        for (int i = 0; i < _perkSlots.Length; i++)
        {
            PerkBuffSlotUI slot = _perkSlots[i];

            if (null == slot)
            {
                continue;
            }

            if (i < buffCount)
            {
                slot.Bind(perkBuffs[i]);
            }
            else
            {
                slot.Hide();
            }
        }

        if (_perkSlots.Length < buffCount)
        {
            Logger.LogWarning($"퍽 슬롯이 모자랍니다. 슬롯 : {_perkSlots.Length}, 퍽 : {buffCount}");
        }
    }
}
