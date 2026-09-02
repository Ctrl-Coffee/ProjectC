using System.Collections.Generic;
using UnityEngine;

public class AutoBattleDropSpawner : MonoBehaviour
{
    [SerializeField] private AutoBattleDropTable _dropTable;
    [SerializeField] private AutoBattleDrop _dropPrefab;

    [SerializeField] private Vector2 _spawnWorldOffset;

    [Header("드랍 개수")]
    [SerializeField] private int _minDropCount = 2;
    [SerializeField] private int _maxDropCount = 3;
    [SerializeField] private float _dropDelayStep = 0.08f;

    [SerializeField] private bool _playArriveSound = true;

    [Header("정산")]
    [SerializeField] private AutoBattleRewardBox _rewardBox;
    [SerializeField] private long _rewardPerIcon = 1;

    private readonly HashSet<string> _warnedMessages = new HashSet<string>();

    public void Spawn(Vector3 worldPosition, AutoBattleUnit target, int sortingOrder)
    {
        if (false == HasRequiredReferences() || null == target)
        {
            return;
        }

        Vector3 spawnPosition = worldPosition;
        spawnPosition.x += _spawnWorldOffset.x;
        spawnPosition.y += _spawnWorldOffset.y;

        int dropCount = Random.Range(_minDropCount, _maxDropCount + 1);

        for (int i = 0; i < dropCount; i++)
        {
            AutoBattleDropTable.Entry picked;

            if (false == _dropTable.TryPick(out picked))
            {
                continue;
            }

            AutoBattleDrop drop = Instantiate(_dropPrefab, transform);

            drop.Play(picked.Icon, picked.CurrencyType, spawnPosition, target.GetCenterPosition(), sortingOrder, i * _dropDelayStep, OnDropArrived);
        }
    }

    private void OnDropArrived(CurrencyType currencyType)
    {
        if (_playArriveSound && null != GameManager.Instance)
        {
            GameManager.Sound.PlaySFX(AddressablePath.Audio.CURRENCY_GAIN);
        }

        if (null == _rewardBox)
        {
            WarnOnce("정산 상자가 지정되지 않아 자동전투 재화가 쌓이지 않습니다.");
            return;
        }

        _rewardBox.AddReward(currencyType, _rewardPerIcon);
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

        if (null == _dropTable)
        {
            WarnOnce("드랍 테이블이 지정되지 않았습니다.");
            return false;
        }

        return true;
    }
}
