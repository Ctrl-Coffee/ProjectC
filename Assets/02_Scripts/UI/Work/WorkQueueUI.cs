using TMPro;
using UnityEngine;

public class WorkQueueUI : MonoBehaviour
{
    private const float REFRESH_INTERVAL = 1f;

    [SerializeField] private WorkQueueSlotUI[] _slots;
    [SerializeField] private TextMeshProUGUI _txtTotalTime;

    private float _refreshTimer = 0f;

    private void OnEnable()
    {
        BindSlots();
        Refresh();
    }

    private void OnDisable()
    {
        UnbindSlots();
    }

    private void Update()
    {
        _refreshTimer += Time.unscaledDeltaTime;

        if (_refreshTimer < REFRESH_INTERVAL)
        {
            return;
        }

        _refreshTimer = 0f;
        Refresh();
    }

    public void Refresh()
    {
        RefreshTotalTime();
        RefreshSlots();
    }

    private void BindSlots()
    {
        if (null == _slots)
        {
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (null == _slots[i])
            {
                continue;
            }

            _slots[i].Bind(i, OnClickSlot);
        }
    }

    private void UnbindSlots()
    {
        if (null == _slots)
        {
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (null == _slots[i])
            {
                continue;
            }

            _slots[i].Unbind();
        }
    }

    private void OnClickSlot(int index)
    {
        if (!AutoWorkQueue.TryCancel(index))
        {
            return;
        }

        Refresh();
    }

    private void RefreshTotalTime()
    {
        if (null == _txtTotalTime)
        {
            return;
        }

        _txtTotalTime.text = Utils.FormatClock(AutoWorkQueue.GetTotalRemainSeconds());
    }

    private void RefreshSlots()
    {
        if (null == _slots)
        {
            return;
        }

        for (int i = 0; i < _slots.Length; i++)
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

            _slots[i].SetProgress(AutoWorkQueue.GetProgress(i));
        }
    }
}
