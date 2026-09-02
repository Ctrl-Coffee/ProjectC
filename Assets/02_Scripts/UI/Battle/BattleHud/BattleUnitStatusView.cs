using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnitStatusView: MonoBehaviour
{
    private readonly BattleUnitStatusViewModel _battleUnitStatusViewModel = new BattleUnitStatusViewModel();

    [SerializeField] private UIButtonComponent _useSignatureSkillButton;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _hpFillImage;
    [SerializeField] private Image _skillFillImage;

    private void Awake()
    {
        UnityUtility.ValidateReference(_useSignatureSkillButton, nameof(_useSignatureSkillButton));
        UnityUtility.ValidateReference(_portraitImage, nameof(_portraitImage));
        UnityUtility.ValidateReference(_hpFillImage, nameof(_hpFillImage));
        UnityUtility.ValidateReference(_skillFillImage, nameof(_skillFillImage));

        _battleUnitStatusViewModel.PropertyChanged += OnPropertyChanged;
    }

    private void OnEnable()
    {
        _useSignatureSkillButton.BindButtonEvent(HandleUseSignatureSkillButton);
        _battleUnitStatusViewModel.Refresh();
    }

    private void OnDisable()
    {
        _useSignatureSkillButton.UnBindButtonAllEvent();
    }

    private void OnDestroy()
    {
        _battleUnitStatusViewModel.PropertyChanged -= OnPropertyChanged;
        _battleUnitStatusViewModel.Dispose();
    }

    public void SetModel(BattleUnitModelBase baseBattleUnitModel)
    {
        _battleUnitStatusViewModel.SetModel(baseBattleUnitModel);
    }

    public void UpdatePortraitSprite(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return;
        }

        BattlePortraitData battlePortraitData = GameManager.DataTable.GetBattlePortraitData(id);

        string portraitKey = battlePortraitData.SpriteAddressableKey;

        Sprite sprite = GameManager.Resource.GetLoadedAsset<Sprite>(portraitKey);

        _portraitImage.sprite = sprite;
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
    }

    public void UpdateSkillBar(bool _IsReady)
    {
        if (_IsReady)
        {
            _skillFillImage.fillAmount = 1.0f;
            return;
        }

        FillSkillBar(_battleUnitStatusViewModel.CalculatedSignatureSkillCooldown);
    }

    private void FillSkillBar(float duration)
    {
        _skillFillImage.DOKill();
        _skillFillImage.fillAmount = 0f;
        _skillFillImage.DOFillAmount(1f, duration);
    }

    private void HandleUseSignatureSkillButton()
    {
        if (!_battleUnitStatusViewModel.IsSignatureSkillReady)
        {
            return;
        }

        if (!GameManager.Battle.RequestCheckPlayerViewIdle(_battleUnitStatusViewModel.BattlePosition))
        {
            return;
        }

        GameManager.Battle.RequestUnitViewUseSignature(_battleUnitStatusViewModel.BattlePosition);
    }

    private void OnPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_battleUnitStatusViewModel.Id):
                UpdatePortraitSprite(_battleUnitStatusViewModel.Id);
                break;
            case nameof(_battleUnitStatusViewModel.Hp):
                UpdateHpBar(_battleUnitStatusViewModel.Hp, _battleUnitStatusViewModel.MaxHp);
                break;
            case nameof(_battleUnitStatusViewModel.IsSignatureSkillReady):
                UpdateSkillBar(_battleUnitStatusViewModel.IsSignatureSkillReady);
                break;
        }
    }
}
