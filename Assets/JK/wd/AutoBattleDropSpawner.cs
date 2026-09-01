using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoBattleDropSpawner : MonoBehaviour
{
    private const float PUNCH_SCALE = 0.3f;
    private const float PUNCH_DURATION = 0.3f;
    private const int PUNCH_VIBRATO = 6;
    private const float PUNCH_ELASTICITY = 0.6f;

    [SerializeField] private AutoBattleDropTable _dropTable;
    [SerializeField] private AutoBattleDrop _dropPrefab;
    [SerializeField] private RectTransform _dropRoot;
    [SerializeField] private Camera _worldCamera;

    [SerializeField] private Vector2 _spawnWorldOffset;

    [Header("드랍 개수")]
    [SerializeField] private int _minDropCount = 2;
    [SerializeField] private int _maxDropCount = 3;
    [SerializeField] private float _dropDelayStep = 0.08f;

    [SerializeField] private bool _playArriveSound = true;

    private Canvas _canvas;
    private readonly HashSet<string> _warnedMessages = new HashSet<string>();

    public void Spawn(Vector3 worldPosition)
    {
        if (false == HasRequiredReferences())
        {
            return;
        }

        Vector3 spawnPosition = worldPosition;
        spawnPosition.x += _spawnWorldOffset.x;
        spawnPosition.y += _spawnWorldOffset.y;

        Vector3 startPosition;

        if (false == TryGetCanvasPosition(spawnPosition, out startPosition))
        {
            WarnOnce("월드 좌표를 캔버스 좌표로 변환하지 못했습니다.");
            return;
        }

        int dropCount = Random.Range(_minDropCount, _maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            AutoBattleDropTable.Entry picked;

            if (false == _dropTable.TryPick(out picked))
            {
                continue;
            }

            // TODO : 보상 지급을 붙일 때 picked 의 재화 종류와 획득량을 여기서 넘긴다.

            CurrencyIconAnchor anchor = CurrencyIconAnchor.Find(picked.CurrencyType);

            if (null == anchor)
            {
                WarnOnce($"{picked.CurrencyType} 재화바가 없습니다.");
                continue;
            }

            Sprite icon = GetAnchorSprite(anchor);

            if (null == icon)
            {
                continue;
            }

            AutoBattleDrop drop = Instantiate(_dropPrefab, _dropRoot);

            drop.Play(icon, picked.CurrencyType, startPosition, anchor.Rect, i * _dropDelayStep, OnDropArrived);
        }
    }

    private void WarnOnce(string message)
    {
        if (false == _warnedMessages.Add(message))
        {
            return;
        }

        Logger.LogWarning(message);
    }

    private bool HasRequiredReferences()
    {
        if (null == _dropPrefab)
        {
            WarnOnce("드랍 프리팹이 지정되지 않았습니다.");
            return false;
        }

        if (null == _dropRoot)
        {
            WarnOnce("드랍 부모가 지정되지 않았습니다.");
            return false;
        }

        if (null == _dropTable)
        {
            WarnOnce("드랍 테이블이 지정되지 않았습니다.");
            return false;
        }

        return true;
    }

    private Sprite GetAnchorSprite(CurrencyIconAnchor anchor)
    {
        Image image = anchor.Rect.GetComponent<Image>();

        if (null == image)
        {
            WarnOnce($"{anchor.CurrencyType} 재화바에 이미지가 없습니다.");
            return null;
        }

        return image.sprite;
    }

    private void OnDropArrived(CurrencyType currencyType)
    {
        if (_playArriveSound && null != GameManager.Instance)
        {
            GameManager.Sound.PlaySFX(AddressablePath.Audio.CURRENCY_GAIN);
        }

        CurrencyIconAnchor anchor = CurrencyIconAnchor.Find(currencyType);

        if (null == anchor)
        {
            return;
        }

        anchor.Rect.DOKill(complete: true);
        anchor.Rect.localScale = Vector3.one;

        Tween tween = anchor.Rect.DOPunchScale(Vector3.one * PUNCH_SCALE, PUNCH_DURATION, PUNCH_VIBRATO, PUNCH_ELASTICITY);

        tween.SetUpdate(true);
    }

    private bool TryGetCanvasPosition(Vector3 worldPosition, out Vector3 canvasPosition)
    {
        canvasPosition = Vector3.zero;

        Camera worldCamera = _worldCamera;

        if (null == worldCamera)
        {
            worldCamera = Camera.main;
        }

        if (null == worldCamera)
        {
            return false;
        }

        if (null == _canvas)
        {
            _canvas = _dropRoot.GetComponentInParent<Canvas>();
        }

        if (null == _canvas)
        {
            return false;
        }

        Vector3 screenPoint = worldCamera.WorldToScreenPoint(worldPosition);

        Camera uiCamera = null;

        if (RenderMode.ScreenSpaceOverlay != _canvas.renderMode)
        {
            uiCamera = _canvas.worldCamera;
        }

        return RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)_canvas.transform, screenPoint, uiCamera, out canvasPosition);
    }
}
