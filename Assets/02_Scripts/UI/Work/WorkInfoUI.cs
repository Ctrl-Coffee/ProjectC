using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkInfoUI : UIBase
{
    private enum WorkCategory
    {
        Manual,
        Auto,
    }

    // TODO: 데이터 테이블로 옮기기
    private const string MANUAL_WORK_ID = "SubtitleEdit";
    private const string MANUAL_WORK_NAME = "Manuel_work1";

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

    private void OnEnable()
    {
        BindButton(_btnClose, OnClickCloseButton, nameof(_btnClose));
        BindButton(_btnManual, OnClickManualTab, nameof(_btnManual));
        BindButton(_btnAuto, OnClickAutoTab, nameof(_btnAuto));

        RefreshWorkList(WorkCategory.Manual);
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
        RefreshWorkList(WorkCategory.Manual);
    }

    private void OnClickAutoTab()
    {
        RefreshWorkList(WorkCategory.Auto);
    }

    private void OnClickManualWork(string workId)
    {
        _workHandler.StartMiniGameAsync().Forget();
    }

    private void OnClickAutoWork(string workId)
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

    private void RefreshWorkList(WorkCategory category)
    {
        ClearWorkList();

        if (WorkCategory.Manual == category)
        {
            SpawnWorkSlot(MANUAL_WORK_ID, MANUAL_WORK_NAME, string.Empty, OnClickManualWork);
            return;
        }

        foreach (AutoWorkData data in GameManager.DataTable.AutoWorkDataTable.Values)
        {
            SpawnWorkSlot(data.Id, data.Name, Utils.FormatDuration(data.DurationSeconds), OnClickAutoWork);
        }
    }

    private void ClearWorkList()
    {
        for (int i = 0; i < _spawnedSlots.Count; i++)
        {
            if (null == _spawnedSlots[i])
            {
                continue;
            }

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

        slot.Setup(workId, workName, info, onClickPlay);
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

        button.UnBindAllButtonEvent();
    }
}
