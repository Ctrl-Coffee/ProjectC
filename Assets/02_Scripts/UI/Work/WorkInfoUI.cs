using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkInfoUI : UIBase
{
    [SerializeField] private UIButtonComponent _btnClose;

    [Header("필터")]
    [SerializeField] private UIButtonComponent _btnManual;
    [SerializeField] private UIButtonComponent _btnAuto;

    [Header("업무 목록")]
    [SerializeField] private RectTransform _workMenuContent;
    [SerializeField] private WorkSlotUI _workSlotPrefab;

    [Header("연출")]
    [SerializeField] private RectTransform _screen;

    [Header("자동업무 큐")]
    [SerializeField] private WorkQueueUI _workQueue;

    private MiniGameFlowHandler _workHandler = new();
    private TypingSoundLoop _typingSound = new();
    private List<WorkSlotUI> _spawnedSlots = new();

    private WorkType _currentWorkType;
    private bool _isWorkListBuilt = false;

    private const float RISE_DURATION = 0.25f;
    private const float FALL_DURATION = 0.2f;
    private const float SCREEN_DELAY = 0.05f;
    private const float SCREEN_LINE_SCALE = 0.02f;
    private const float SCREEN_ON_LINE = 0.1f;
    private const float SCREEN_ON_EXPAND = 0.14f;
    private const float SCREEN_OFF_COLLAPSE = 0.14f;
    private const float SCREEN_OFF_SHRINK = 0.1f;

    private Vector2 _panelShownPosition;
    private bool _isPositionCaptured = false;

    private void Awake()
    {
        CapturePanelPosition();
    }

    public override Tween PlayOpenAnimation()
    {
        CapturePanelPosition();

        if (!IsPlayAnimation)
        {
            ResetToShown();
            return null;
        }

        _panel.DOKill();
        _panel.localScale = Vector3.one;
        _panel.anchoredPosition = GetHiddenPosition();

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        sequence.Append(_panel.DOAnchorPos(_panelShownPosition, RISE_DURATION).SetEase(Ease.OutCubic));

        if (null == _screen)
        {
            return sequence;
        }

        _screen.DOKill();
        _screen.localScale = new Vector3(0f, SCREEN_LINE_SCALE, 1f);

        sequence.AppendInterval(SCREEN_DELAY);
        sequence.Append(_screen.DOScaleX(1f, SCREEN_ON_LINE).SetEase(Ease.OutQuad));
        sequence.Append(_screen.DOScaleY(1f, SCREEN_ON_EXPAND).SetEase(Ease.OutQuad));

        return sequence;
    }

    public override Tween PlayCloseAnimation()
    {
        CapturePanelPosition();

        _panel.DOKill();

        Sequence sequence = DOTween.Sequence().SetUpdate(true);

        if (null != _screen)
        {
            _screen.DOKill();

            sequence.Append(_screen.DOScaleY(SCREEN_LINE_SCALE, SCREEN_OFF_COLLAPSE).SetEase(Ease.InQuad));
            sequence.Append(_screen.DOScaleX(0f, SCREEN_OFF_SHRINK).SetEase(Ease.InQuad));
        }

        sequence.Append(_panel.DOAnchorPos(GetHiddenPosition(), FALL_DURATION).SetEase(Ease.InCubic));

        return sequence;
    }

    private void CapturePanelPosition()
    {
        if (_isPositionCaptured || null == _panel)
        {
            return;
        }

        _panelShownPosition = _panel.anchoredPosition;
        _isPositionCaptured = true;
    }

    private Vector2 GetHiddenPosition()
    {
        if (null == _panel)
        {
            return _panelShownPosition;
        }

        float distance = _panelShownPosition.y + _panel.rect.height;

        return new Vector2(_panelShownPosition.x, _panelShownPosition.y - distance);
    }

    private void ResetToShown()
    {
        if (null != _panel)
        {
            _panel.DOKill();
            _panel.anchoredPosition = _panelShownPosition;
            _panel.localScale = Vector3.one;
        }

        if (null != _screen)
        {
            _screen.DOKill();
            _screen.localScale = Vector3.one;
        }
    }

    private void OnEnable()
    {
        BindButton(_btnClose, OnClickCloseButton, nameof(_btnClose));
        BindButton(_btnManual, OnClickManualTab, nameof(_btnManual));
        BindButton(_btnAuto, OnClickAutoTab, nameof(_btnAuto));

        RefreshTabs();

        _isWorkListBuilt = false;

        RefreshWorkList(WorkType.Manual);

        _typingSound.Play();
    }

    private void OnDisable()
    {
        UnbindButton(_btnClose);
        UnbindButton(_btnManual);
        UnbindButton(_btnAuto);

        _typingSound.Stop();
    }

    private void OnDestroy()
    {
        _workHandler.Cancel();
    }

    private void OnClickManualTab()
    {
        RefreshWorkList(WorkType.Manual);
    }

    private void OnClickAutoTab()
    {
        RefreshWorkList(WorkType.Auto);
    }

    private void RefreshTabs()
    {
        SetTabInteractable(_btnManual, WorkType.Manual);
        SetTabInteractable(_btnAuto, WorkType.Auto);
    }

    private void SetTabInteractable(UIButtonComponent button, WorkType workType)
    {
        if (null == button)
        {
            return;
        }

        button.SetInteractable(WorkTable.HasAny(workType));
    }

    private void OnClickWork(string workId)
    {
        WorkData data = GameManager.DataTable.GetWorkData(workId);

        if (null == data)
        {
            Logger.LogError($"업무 데이터를 찾을 수 없습니다. id: {workId}");
            return;
        }

        if (WorkType.Auto == data.Type)
        {
            EnqueueAutoWork(data.Id);
            return;
        }

        RunMiniGameAsync(data).Forget();
    }

    private async UniTaskVoid RunMiniGameAsync(WorkData data)
    {
        _typingSound.Stop();

        try
        {
            await _workHandler.StartMiniGameAsync(data);
        }
        finally
        {
            if (isActiveAndEnabled)
            {
                _typingSound.Play();
            }
        }
    }

    private void EnqueueAutoWork(string workId)
    {
        if (!AutoWorkQueue.TryEnqueue(workId))
        {
            return;
        }

        if (null == _workQueue)
        {
            return;
        }

        _workQueue.Refresh();
    }

    private void RefreshWorkList(WorkType workType)
    {
        if (_isWorkListBuilt && _currentWorkType == workType)
        {
            return;
        }

        _currentWorkType = workType;
        _isWorkListBuilt = true;

        ClearWorkList();

        IReadOnlyList<WorkData> workList = WorkTable.GetList(workType);

        for (int i = 0; i < workList.Count; i++)
        {
            WorkData data = workList[i];

            SpawnWorkSlot(data.Id, data.Name, GetSlotInfo(data), data.IconKey, OnClickWork);
        }
    }

    private string GetSlotInfo(WorkData data)
    {
        bool hasDescription = !Utils.IsNullOrWhiteSpace(data.Description);

        if (WorkType.Auto != data.Type)
        {
            if (!hasDescription)
            {
                return string.Empty;
            }

            return data.Description;
        }

        string duration = Utils.FormatDuration(data.DurationSeconds);

        if (!hasDescription)
        {
            return duration;
        }

        return $"{data.Description} ({duration})";
    }

    private void ClearWorkList()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (null == _spawnedSlots[i])
            {
                continue;
            }

            _spawnedSlots[i].Unbind();
            Destroy(_spawnedSlots[i].gameObject);
        }

        _spawnedSlots.Clear();
    }

    private void SpawnWorkSlot(string workId, string workName, string info, string iconKey, Action<string> onClickPlay)
    {
        if (null == _workSlotPrefab || null == _workMenuContent)
        {
            Logger.LogError("WorkSlot 프리팹 또는 Content가 연결되지 않았습니다.");
            return;
        }

        WorkSlotUI slot = Instantiate(_workSlotPrefab, _workMenuContent, false);

        slot.Bind(workId, onClickPlay);
        slot.SetInfo(workName, info);
        slot.SetIcon(iconKey);

        _spawnedSlots.Add(slot);
    }

    private void BindButton(UIButtonComponent button, Action onClick, string fieldName)
    {
        if (null == button)
        {
            Logger.LogError($"{fieldName}이 연결되지 않았습니다.");
            return;
        }

        button.BindButtonEvent(onClick);
    }

    private void UnbindButton(UIButtonComponent button)
    {
        if (null == button)
        {
            return;
        }

        button.UnBindButtonAllEvent();
    }
}
