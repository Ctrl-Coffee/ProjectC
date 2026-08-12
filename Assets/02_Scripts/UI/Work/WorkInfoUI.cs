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
    [SerializeField] private RectTransform _content;
    [SerializeField] private WorkSlotUI _workSlotPrefab;

    [Header("자동업무 큐")]
    [SerializeField] private WorkQueueUI _workQueue;

    private MiniGameFlowHandler _workHandler = new();
    private List<WorkSlotUI> _spawnedSlots = new();

    private WorkType _currentWorkType;
    private bool _isWorkListBuilt = false;

    private void OnEnable()
    {
        BindButton(_btnClose, OnClickCloseButton, nameof(_btnClose));
        BindButton(_btnManual, OnClickManualTab, nameof(_btnManual));
        BindButton(_btnAuto, OnClickAutoTab, nameof(_btnAuto));

        RefreshWorkList(WorkType.Manual);
    }

    private void OnDisable()
    {
        UnbindButton(_btnClose);
        UnbindButton(_btnManual);
        UnbindButton(_btnAuto);
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

        _workHandler.StartMiniGameAsync(data).Forget();
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

            SpawnWorkSlot(data.Id, data.Name, GetSlotInfo(data), OnClickWork);
        }
    }

    private string GetSlotInfo(WorkData data)
    {
        if (WorkType.Auto != data.Type)
        {
            return string.Empty;
        }

        return Utils.FormatDuration(data.DurationSeconds);
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

    private void SpawnWorkSlot(string workId, string workName, string info, Action<string> onClickPlay)
    {
        if (null == _workSlotPrefab || null == _content)
        {
            Logger.LogError("WorkSlot 프리팹 또는 Content가 연결되지 않았습니다.");
            return;
        }

        WorkSlotUI slot = Instantiate(_workSlotPrefab, _content, false);

        slot.Bind(workId, onClickPlay);
        slot.SetInfo(workName, info);

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
