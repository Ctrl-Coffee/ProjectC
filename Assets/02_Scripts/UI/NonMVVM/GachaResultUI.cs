using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultUI : UIBase
{
    [SerializeField] private Transform _slotRoot;
    [SerializeField] private Button _closeButton;

    private const float _slotInterval = 0.05f;

    private Tween _openTween;

    private List<GachaResultSlotUI> _slot = new();

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnClickCloseButton);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnClickCloseButton);
    }
    public void Init(IReadOnlyList<GachaResultData> results)
    {
        RemoveAllSlot();

        if (results == null) return;

        foreach (GachaResultData result in results)
        {
            CreateSlot(result);
        }

        if (_openTween == null || _openTween.IsActive() == false)
        {
            PlaySlotAnimations();
            return;
        }

        _openTween.OnComplete(PlaySlotAnimations);
    }

    private void PlaySlotAnimations()
    {
        for (int index = 0; index < _slot.Count; index++)
        {
            _slot[index].PlayOpenAnimation()?.SetDelay(index * 0.1f);
        }
    }
    private void CreateSlot(GachaResultData result)
    {
        GameObject slotPrefab = GameManager.Resource.GetLoadedAsset<GameObject>(AddressablePath.GetUIPath(typeof(GachaResultSlotUI)));
        if (slotPrefab == null)
        {
            Debug.LogError("결과 슬롯 프리팹을 찾을수 없습니다");
            return;
        }

        GameObject slotInstance = Instantiate(slotPrefab, _slotRoot);
        if (slotInstance == null) return;

        var slotComponent = slotInstance.GetComponent<GachaResultSlotUI>();
        if (slotComponent == null) return;

        slotComponent.Init(result);
        slotComponent.Hide();

        _slot.Add(slotComponent);
    }

    private void RemoveAllSlot()
    {
        foreach (GachaResultSlotUI slot in _slot)
        {
            Destroy(slot.gameObject);
        }
        
        _slot.Clear();
    }

    public override Tween PlayOpenAnimation()
    {
        _openTween = base.PlayOpenAnimation();
        return _openTween;
    }
    public override Tween PlayCloseAnimation()
    {
        for (int index = 0; index < _slot.Count; index++)
        {
            _slot[index].PlayCloseAnimation().SetDelay(index * _slotInterval);
        }

        float slotTotalTime = _slot.Count > 0
            ? GachaResultSlotUI.CloseDuration + _slotInterval * (_slot.Count - 1)
            : 0f;

        return base.PlayCloseAnimation().SetDelay(slotTotalTime);
    }
}
