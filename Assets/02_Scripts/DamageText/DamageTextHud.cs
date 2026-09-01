using System.Collections.Generic;
using UnityEngine;

public class DamageTextHud : UIBase
{
    [SerializeField] private DamageText _damageTextPrefab;
    [SerializeField] private RectTransform _damageTextRoot;

    private readonly Queue<DamageText> _damageTextPool = new();
    private readonly HashSet<DamageText> _activeDamageTexts = new();

    private void Awake()
    {
        UnityUtility.ValidateReference(_damageTextPrefab, nameof(_damageTextPrefab));
        UnityUtility.ValidateReference(_damageTextRoot, nameof(_damageTextRoot));

        InitializeDamageTextPool();
    }

    private void OnDisable()
    {
        ReturnAllDamageTexts();
    }

    private void OnDestroy()
    {
        DestroyAllDamageTexts();
    }

    public void ShowDamageText(DamageResult damageResult, Vector2 startPosition)
    {
        DamageText damageText = GetDamageText();

        Vector2 screenPosition = Camera.main.WorldToScreenPoint(startPosition);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(_damageTextRoot, screenPosition, null, out Vector2 localPosition);

        localPosition += GetRandomOffset();

        damageText.Show(damageResult, localPosition);

        _activeDamageTexts.Add(damageText);
    }

    public void HideDamageText(DamageText damageText)
    {
        if (!_activeDamageTexts.Remove(damageText))
        {
            return;
        }

        ReturnDamageText(damageText);
    }


    private void InitializeDamageTextPool()
    {
        for (int i = 0; i < Const.DAMAGE_TEXT_POOL_INITIAL_SIZE; i++)
        {
            _damageTextPool.Enqueue(CreateDamageText());
        }
    }

    private DamageText GetDamageText()
    {
        if (_damageTextPool.Count == 0)
        {
            return CreateDamageText();
        }

        DamageText damageText = _damageTextPool.Dequeue();
        damageText.gameObject.SetActive(true);

        return damageText;
    }

    private DamageText CreateDamageText()
    {
        DamageText damageText = Instantiate(_damageTextPrefab, _damageTextRoot);
        damageText.gameObject.SetActive(false);

        return damageText;
    }

    private Vector2 GetRandomOffset()
    {
        Vector2 randomOffset = new Vector2
            (
            Random.Range(-Const.DAMAGE_TEXT_HORIZONTAL_RANDOM_OFFSET, Const.DAMAGE_TEXT_HORIZONTAL_RANDOM_OFFSET),
            Random.Range(Const.DAMAGE_TEXT_VERTICAL_MIN_OFFSET, Const.DAMAGE_TEXT_VERTICAL_MAX_OFFSET)
            );

        return randomOffset;
    }

    private void ReturnAllDamageTexts()
    {
        foreach (DamageText damageText in _activeDamageTexts)
        {
            damageText.gameObject.SetActive(false);
            _damageTextPool.Enqueue(damageText);
        }

        _activeDamageTexts.Clear();
    }

    private void ReturnDamageText(DamageText damageText)
    {
        damageText.gameObject.SetActive(false);
        _damageTextPool.Enqueue(damageText);
    }

    private void DestroyAllDamageTexts()
    {
        foreach (DamageText damageText in _activeDamageTexts)
        {
            if (damageText != null)
            {
                Destroy(damageText.gameObject);
            }
        }

        _activeDamageTexts.Clear();

        while (_damageTextPool.Count > 0)
        {
            DamageText damageText = _damageTextPool.Dequeue();

            if (damageText != null)
            {
                Destroy(damageText.gameObject);
            }
        }
    }
}