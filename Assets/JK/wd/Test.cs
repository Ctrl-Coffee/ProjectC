using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] private TestUnit _player;
    [SerializeField] private TestUnit[] _enemies;

    [Header("Background")]
    [SerializeField] private TestBackground _background;

    [Header("Enemy Movement")]
    [SerializeField] private float _enemyApproachDistance = 3f;
    [SerializeField] private float _enemyApproachDuration = 3f;

    [Header("Attack")]
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private float _hitDelay = 0.2f;
    [SerializeField] private float _deathDelay = 0.5f;

    [Header("Loop")]
    [SerializeField] private float _nextWaveDelay = 1f;


    private int _spawnedCount;
    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        Initialize();
        PlayLoopAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        Stop();
    }

    private void Initialize()
    {
        _player.Initialize();

        foreach (TestUnit enemy in _enemies)
        {
            enemy.Initialize();
        }

        Debug.Log("플레이어 Idle");
        //_player.PlayIdle();

        foreach (TestUnit enemy in _enemies)
        {
            Debug.Log("적 Idle");
            //enemy.PlayIdle();
        }
    }

    private async UniTaskVoid PlayLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                await PlayEncounter(cancellationToken);

                ResetEnemies();

                await UniTask.Delay(Mathf.RoundToInt(_nextWaveDelay * 1000f), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _background.StopScroll();
        }
    }

    private async UniTask PlayEncounter(CancellationToken cancellationToken)
    {
        await ApproachEnemies(cancellationToken);

        for (int count = 5; count > 0; count--)
        {
            await PlayAttack(cancellationToken);
        }

        await PlayDeath(cancellationToken);
    }

    private async UniTask ApproachEnemies(
        CancellationToken cancellationToken)
    {
        foreach (TestUnit enemy in _enemies)
        {
            Debug.Log("적 Walk");
           // enemy.PlayWalk();
        }

        UniTask[] moveTasks = new UniTask[_enemies.Length];

        for (int i = 0; i < _enemies.Length; i++)
        {
            float yOffset = UnityEngine.Random.Range(-0.2f, 0.2f);

            Vector3 targetPosition = new Vector3(_player.transform.position.x + _enemyApproachDistance, _enemies[i].transform.position.y + yOffset, _enemies[i].transform.position.z);

            moveTasks[i] = _enemies[i].MoveTo(targetPosition, _enemyApproachDuration);
        }

        await UniTask.WhenAll(moveTasks);

        cancellationToken.ThrowIfCancellationRequested();

        foreach (TestUnit enemy in _enemies)
        {
            Debug.Log("적 Idle");
           // enemy.PlayIdle();
        }
    }

    private async UniTask PlayAttack(CancellationToken cancellationToken)
    {
        _background.StopScroll();

        Debug.Log("플레이어 타격");
        //_player.PlayAttack();

        await UniTask.Delay(Mathf.RoundToInt(_attackDelay * 1000f), cancellationToken: cancellationToken);

        foreach (TestUnit enemy in _enemies)
        {
            Debug.Log("적 타격");
            //enemy.PlayHit();
        }

        await UniTask.Delay(Mathf.RoundToInt(_hitDelay * 1000f), cancellationToken: cancellationToken);
    }

    private async UniTask PlayDeath(CancellationToken cancellationToken)
    {
        foreach (TestUnit enemy in _enemies)
        {
            Debug.Log("적 Death");
            //enemy.PlayDeath();
        }

        Debug.Log("보상 흭득");

        await UniTask.Delay(Mathf.RoundToInt(_deathDelay * 1000f), cancellationToken: cancellationToken);

        _background.StartScroll();
    }

    private void ResetEnemies()
    {
        int enemyCount = UnityEngine.Random.Range(1, 4);

        for (int i = 0; i < _enemies.Length; i++)
        {
            _enemies[i].ResetPosition();
            _enemies[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < enemyCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, _enemies.Length);

            while (_enemies[randomIndex].gameObject.activeSelf)
            {
                randomIndex = UnityEngine.Random.Range(0, _enemies.Length);
            }

            _enemies[randomIndex].gameObject.SetActive(true);
        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        _player.Stop();

        foreach (TestUnit enemy in _enemies)
        {
            enemy.Stop();
        }

        _background.StopScroll();
    }
}
