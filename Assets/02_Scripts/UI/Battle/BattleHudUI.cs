using System.Collections.Generic;
using UnityEngine;

public class BattleHudUI : UIBase
{
    [SerializeField] private DamageTextUI _damageTextPrefab;
    [SerializeField] private Transform _damageTextRoot;

    private List<DamageTextUI> _instances = new();

    //TODO 전투 캐릭터가 월드/UI인지 확인 후 좌표 방식 확정
    public void ShowDamage(Vector3 worldPosition, long damage)
    {
        Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPosition);
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)_damageTextRoot, screenPoint, null, out Vector2 localPoint);

        DamageTextUI damageText = GetInstance();
        RectTransform rect = (RectTransform)damageText.transform;
        rect.anchoredPosition = localPoint;

        damageText.SetDamage(damage);
        damageText.gameObject.SetActive(true);
        damageText.PlayDamage();
    }

    private DamageTextUI GetInstance()
    {
        foreach (DamageTextUI instance in _instances)
        {
            if (instance.gameObject.activeSelf == false)
            {
                return instance;
            }
        }

        DamageTextUI newInstance = Instantiate(_damageTextPrefab, _damageTextRoot);
        _instances.Add(newInstance);
        return newInstance;
    }
    
}
