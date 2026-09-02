using UnityEngine;

public class AutoBattleRewardBox : MonoBehaviour
{
    [SerializeField] private BackgroundInputHandler _inputHandler;
    [SerializeField] private Collider2D _collider;

    [Header("클릭 좌표 변환용 카메라")]
    [SerializeField] private Camera _worldCamera;

    [Header("이펙트")]
    [SerializeField] private GameObject _readyMark;

    private readonly AutoBattlePendingReward _pending = new AutoBattlePendingReward();
    private readonly AutoBattlePendingReward _settling = new AutoBattlePendingReward();

    private void OnEnable()
    {
        RefreshReadyMark();

        if (null == _inputHandler)
        {
            Logger.LogError("입력 핸들러가 연결되지 않아 클릭을 받을 수 없습니다.");
            return;
        }

        _inputHandler.OnTapped += OnTapped;
    }

    private void OnDisable()
    {
        if (null == _inputHandler)
        {
            return;
        }

        _inputHandler.OnTapped -= OnTapped;
    }

    public void AddReward(CurrencyType currencyType, long amount)
    {
        _pending.Add(currencyType, amount);

        RefreshReadyMark();
    }

    private void OnTapped(Vector2 screenPosition)
    {
        if (null == _collider)
        {
            Logger.LogError("콜라이더가 연결되지 않았습니다.");
            return;
        }

        Camera camera = GetCamera();

        if (null == camera)
        {
            return;
        }

        Vector3 worldPosition = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 0f));

        if (false == _collider.OverlapPoint(worldPosition))
        {
            return;
        }

        OpenRewardUI();
    }

    private void OpenRewardUI()
    {
        if (false == _pending.HasAny)
        {
            Logger.Log("정산할 재화가 없습니다.");
            return;
        }

        _pending.MoveTo(_settling);

        RefreshReadyMark();

        if (false == GameManager.UI.OpenAutoBattleRewardUI(_settling, OnPayout))
        {
            _settling.MoveTo(_pending);

            RefreshReadyMark();
        }
    }

    private void OnPayout()
    {
        _settling.Payout();
    }

    private Camera GetCamera()
    {
        if (null != _worldCamera)
        {
            return _worldCamera;
        }

        return Camera.main;
    }

    private void RefreshReadyMark()
    {
        if (null == _readyMark)
        {
            return;
        }

        _readyMark.SetActive(_pending.HasAny);
    }
}
