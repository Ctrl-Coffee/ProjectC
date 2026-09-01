using UnityEngine;
using UnityEngine.UI;

public class BattleUnitHpBarView : MonoBehaviour
{
    [SerializeField] private Image _hpFillImage;

    private BattleUnitHpBarViewModel _battleUnitHpBarViewModel = new BattleUnitHpBarViewModel();

    private void Awake()
    {
        _battleUnitHpBarViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnEnable()
    {
        _battleUnitHpBarViewModel.Refresh();
    }

    private void OnDestroy()
    {
        _battleUnitHpBarViewModel.PropertyChanged -= OnPropertyChanged;
        _battleUnitHpBarViewModel.Dispose();
    }

    public void SetModel(BattleUnitModelBase baseBattleUnitModel)
    {
        _battleUnitHpBarViewModel.SetModel(baseBattleUnitModel);
    }

    public void UpdateHpBar(float currentHp, float maxHp)
    {
        if (maxHp <= 0f)
        {
            gameObject.SetActive(false);
            return;
        }

        float hpRatio = Mathf.Clamp01(currentHp / maxHp);
        _hpFillImage.fillAmount = hpRatio;

        if (currentHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_battleUnitHpBarViewModel.Hp):
                UpdateHpBar(_battleUnitHpBarViewModel.Hp, _battleUnitHpBarViewModel.MaxHp);
                break;
        }
    }
}