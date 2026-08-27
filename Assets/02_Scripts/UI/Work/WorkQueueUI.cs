using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkQueueUI : MonoBehaviour
{
    private const float REFRESH_INTERVAL = 1f;

    [SerializeField] private WorkQueueSlotUI _slotPrefab;

    [SerializeField] private RectTransform _slotRoot;

    [SerializeField] private TextMeshProUGUI _txtTotalTime;

    private List<WorkQueueSlotUI> _slots = new();
    private float _refreshTimer = 0f;
    private bool _isPrefabWarned = false;

    private void OnEnable()
    {
        AutoWorkQueue.OnQueueChanged += Refresh;
        GameManager.Perk.OnPerkChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        AutoWorkQueue.OnQueueChanged -= Refresh;
        GameManager.Perk.OnPerkChanged -= Refresh;

        ClearSlots();
    }

    private void Update()
    {
        RefreshProgress();

        _refreshTimer += Time.unscaledDeltaTime;

        if (_refreshTimer < REFRESH_INTERVAL)
        {
            return;
        }

        _refreshTimer = 0f;
        RefreshTotalTime();
    }

    public void Refresh()
    {
        RebuildSlots();
        RefreshTotalTime();
        RefreshSlotContents();
        RefreshProgress();    
    }

    private void RebuildSlots()
    {
        if (null == _slotPrefab)
        {
            WarnPrefabOnce();
            return;
        }

        int maxSlotCount = AutoWorkQueue.MaxSlotCount;

        if (_slots.Count == maxSlotCount)
        {
            return;
        }

        ClearSlots();

        RectTransform root = null != _slotRoot ? _slotRoot : this.transform as RectTransform;

        for (int i = 0; i < maxSlotCount; i++)
        {
            WorkQueueSlotUI slot = Instantiate(_slotPrefab, root, false);

            slot.Bind(i, OnClickSlot);

            _slots.Add(slot);
        }
    }

    private void WarnPrefabOnce()
    {
        if (_isPrefabWarned)
        {
            return;
        }

        _isPrefabWarned = true;

        Logger.LogError("큐 슬롯 프리팹이 연결되지 않았습니다.");
    }

    private void ClearSlots()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (null == _slots[i])
            {
                continue;
            }

            _slots[i].Unbind();

            Destroy(_slots[i].gameObject);
        }

        _slots.Clear();
    }

    private void OnClickSlot(int index)
    {
        AutoWorkQueue.TryCancel(index);
    }

    private void RefreshTotalTime()
    {
        if (null == _txtTotalTime)
        {
            return;
        }

        _txtTotalTime.text = Utils.FormatClock(AutoWorkQueue.GetTotalRemainSeconds());
    }

    private void RefreshSlotContents()
    {
        for (int i = 0; i < _slots.Count; i++)
        {
            if (null == _slots[i])
            {
                continue;
            }

            if (i >= AutoWorkQueue.Count)
            {
                _slots[i].SetEmpty();
                continue;
            }

            _slots[i].SetIcon(GetIconKey(AutoWorkQueue.GetWorkId(i)));
        }
    }

    private void RefreshProgress()
    {
        int count = AutoWorkQueue.Count;

        for (int i = 0; i < _slots.Count; i++)
        {
            if (null == _slots[i])
            {
                continue;
            }

            if (i >= count)
            {
                continue;
            }

            _slots[i].SetProgress(AutoWorkQueue.GetProgress(i));
        }
    }

    private string GetIconKey(string workId)
    {
        WorkData data = GameManager.DataTable.GetWorkData(workId);

        if (null == data)
        {
            return string.Empty;
        }

        return data.IconKey;
    }
}
