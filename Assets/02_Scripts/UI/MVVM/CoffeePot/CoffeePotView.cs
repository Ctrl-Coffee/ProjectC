using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoffeePotView : MonoBehaviour
{
    [SerializeField] private UIButtonComponent _coffeeBtn;
    [SerializeField] private Image _gauge;
    [SerializeField] private TextMeshProUGUI _remainText;

    private CoffeePotViewModel _viewModel;

    private void OnEnable()
    {
        if (null == _viewModel)
        {
            BindViewModel();
        }

        Subscribe();

        if (null != _coffeeBtn)
        {
            _coffeeBtn.BindButtonEvent(OnClickCoffeePot);
        }

        _viewModel.StartTick();
    }

    private void OnDisable()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.StopTick();

        UnSubscribe();

        if (null != _coffeeBtn)
        {
            _coffeeBtn.UnBindButtonAllEvent();
        }
    }

    private void OnDestroy()
    {
        if (null == _viewModel)
        {
            return;
        }

        UnSubscribe();

        _viewModel.UnBind();
        _viewModel = null;
    }

    public void OnClickCoffeePot()
    {
        if (null == _viewModel)
        {
            return;
        }

        _viewModel.TryUse();
    }

    private void BindViewModel()
    {
        _viewModel = GameManager.ViewModel.CreateCoffeePotViewModel();
    }

    private void Subscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel += OnPropertyChanged;
    }

    private void UnSubscribe()
    {
        _viewModel.OnPropertyChanged_ViewModel -= OnPropertyChanged;
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(CoffeePotViewModel.ChargeProgress):
                SetGauge(_viewModel.ChargeProgress);
                break;
            case nameof(CoffeePotViewModel.RemainText):
                SetRemainText(_viewModel.RemainText);
                break;
            case nameof(CoffeePotViewModel.IsReady):
                SetButtonInteractable(_viewModel.IsReady);
                break;
        }
    }

    private void SetGauge(float chargeProgress)
    {
        if (null == _gauge)
        {
            return;
        }

        _gauge.fillAmount = chargeProgress;
    }

    private void SetRemainText(string remainText)
    {
        if (null == _remainText)
        {
            return;
        }

        bool hasRemain = 0 < remainText.Length;

        if (_remainText.gameObject.activeSelf != hasRemain)
        {
            _remainText.gameObject.SetActive(hasRemain);
        }

        if (hasRemain)
        {
            _remainText.text = remainText;
        }
    }

    private void SetButtonInteractable(bool isReady)
    {
        if (null == _coffeeBtn)
        {
            return;
        }

        _coffeeBtn.SetInteractable(isReady);
    }
}
