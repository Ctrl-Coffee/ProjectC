using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class AutoBattleController : MonoBehaviour
{
    [Header("Units")]
    [SerializeField] private AutoBattleUnit _player;
    [SerializeField] private AutoBattleUnit[] _enemies;

    [Header("Background")]
    [SerializeField] private AutoBattleBackground _background;

    [Header("Enemy Movement")]
    [SerializeField] private float _enemyApproachDistance = 3f;
    [SerializeField] private float _enemyApproachDuration = 3f;
    [SerializeField] private float _enemyVerticalOffset = 0.2f;
    [SerializeField] private float _enemySpacing = 1.2f;

    [Header("Attack")]
    [SerializeField] private int _attackCountPerEnemy = 3;
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private float _hitDelay = 0.2f;
    [SerializeField] private float _deathDelay = 0.5f;

    [Header("Loop")]
    [SerializeField] private float _nextWaveDelay = 1f;

    // 비워두면 컨트롤러의 기본 클립을 쓴다.
    [Header("애니메이션 세트")]
    [SerializeField] private UnitAnimationSet _playerAnimationSet;
    [SerializeField] private UnitAnimationSet[] _enemyAnimationSets;

    private int[] _spawnOrder;
    private CancellationTokenSource _cancellationTokenSource;

    private void OnEnable()
    {
        if (false == HasRequiredReferences())
        {
            return;
        }

        _cancellationTokenSource = new CancellationTokenSource();

        Initialize();
        PlayLoopAsync(_cancellationTokenSource.Token).Forget();
    }

    private void OnDisable()
    {
        Stop();
    }

    private bool HasRequiredReferences()
    {
        if (null == _player)
        {
            Logger.LogError("자동전투 : 플레이어 유닛이 지정되지 않았습니다.");
            return false;
        }

        if (null == _enemies || 0 == _enemies.Length)
        {
            Logger.LogError("자동전투 : 적 유닛이 지정되지 않았습니다.");
            return false;
        }

        if (null == _background)
        {
            Logger.LogError("자동전투 : 배경이 지정되지 않았습니다.");
            return false;
        }

        return true;
    }

    private void Initialize()
    {
        _player.Initialize();
        _player.ApplyAnimationSet(_playerAnimationSet);
        _player.PlayIdle();

        _spawnOrder = new int[_enemies.Length];

        for (int i = 0; i < _enemies.Length; i++)
        {
            _spawnOrder[i] = i;

            if (null == _enemies[i])
            {
                continue;
            }

            _enemies[i].Initialize();
        }

        // runtimeAnimatorController 를 갈아끼우면 상태가 초기화되므로 세트를 먼저 입힌다.
        ApplyEnemyAnimationSets();

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (null == _enemies[i])
            {
                continue;
            }

            _enemies[i].PlayIdle();
        }
    }

    private void ApplyEnemyAnimationSets()
    {
        if (null == _enemyAnimationSets)
        {
            return;
        }

        int count = Mathf.Min(_enemies.Length, _enemyAnimationSets.Length);

        for (int i = 0; i < count; i++)
        {
            if (null == _enemies[i])
            {
                continue;
            }

            _enemies[i].ApplyAnimationSet(_enemyAnimationSets[i]);
        }
    }

    // 스테이지 연동 시 EnemyGroupId 로 찾은 세트를 넘기는 진입점.
    public void SetEnemyAnimationSets(UnitAnimationSet[] animationSets)
    {
        _enemyAnimationSets = animationSets;

        ApplyEnemyAnimationSets();
    }

    private async UniTaskVoid PlayLoopAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ResetEnemies();

                await PlayEncounter(cancellationToken);

                await UniTask.Delay(Mathf.RoundToInt(_nextWaveDelay * 1000f), cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (null != _background)
            {
                _background.StopScroll();
            }
        }
    }

    private async UniTask PlayEncounter(CancellationToken cancellationToken)
    {
        await ApproachEnemies(cancellationToken);

        _background.StopScroll();

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (false == IsAlive(_enemies[i]))
            {
                continue;
            }

            await DefeatEnemy(_enemies[i], cancellationToken);
        }

        _player.PlayIdle();

        _background.StartScroll();
    }

    private async UniTask DefeatEnemy(AutoBattleUnit enemy, CancellationToken cancellationToken)
    {
        for (int i = 0; i < _attackCountPerEnemy; i++)
        {
            await PlayAttack(enemy, cancellationToken);
        }

        await PlayDeath(enemy, cancellationToken);
    }

    private async UniTask ApproachEnemies(CancellationToken cancellationToken)
    {
        UniTask[] moveTasks = new UniTask[_enemies.Length];

        // 슬롯 번호로 계산하면 한 마리만 나왔을 때 혼자 멀리 선다.
        int lineIndex = 0;

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (false == IsAlive(_enemies[i]))
            {
                moveTasks[i] = UniTask.CompletedTask;
                continue;
            }

            float yOffset = UnityEngine.Random.Range(-_enemyVerticalOffset, _enemyVerticalOffset);

            Vector3 targetPosition = new Vector3(
                _player.transform.position.x + _enemyApproachDistance + lineIndex * _enemySpacing,
                _enemies[i].transform.position.y + yOffset,
                _enemies[i].transform.position.z);

            moveTasks[i] = _enemies[i].MoveTo(targetPosition, _enemyApproachDuration);

            lineIndex++;
        }

        await UniTask.WhenAll(moveTasks);

        cancellationToken.ThrowIfCancellationRequested();

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (IsAlive(_enemies[i]))
            {
                _enemies[i].PlayIdle();
            }
        }
    }

    private async UniTask PlayAttack(AutoBattleUnit enemy, CancellationToken cancellationToken)
    {
        _player.PlayAttack();

        // 실제로 검이 닿는 프레임에 피격 반응을 맞춘다.
        await _player.WaitForAttackHitAsync(_attackDelay, cancellationToken);

        enemy.PlayHit();

        await UniTask.Delay(Mathf.RoundToInt(_hitDelay * 1000f), cancellationToken: cancellationToken);
    }

    private async UniTask PlayDeath(AutoBattleUnit enemy, CancellationToken cancellationToken)
    {
        enemy.PlayDeath();

        // TODO : 처치 보상 지급과 드랍 아이콘 연출을 이 지점에 연결한다.

        float deathWaitSeconds = Mathf.Max(_deathDelay, enemy.GetDeathAnimationLength());

        await UniTask.Delay(Mathf.RoundToInt(deathWaitSeconds * 1000f), cancellationToken: cancellationToken);

        enemy.gameObject.SetActive(false);
    }

    private void ResetEnemies()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            if (null == _enemies[i])
            {
                continue;
            }

            _enemies[i].ResetPosition();
            _enemies[i].gameObject.SetActive(false);
        }

        ShuffleSpawnOrder();

        int enemyCount = UnityEngine.Random.Range(1, _enemies.Length + 1);

        for (int i = 0; i < enemyCount; i++)
        {
            AutoBattleUnit enemy = _enemies[_spawnOrder[i]];

            if (null == enemy)
            {
                continue;
            }

            enemy.gameObject.SetActive(true);

            // 직전 웨이브에서 사망 상태로 끝났으므로 Idle 로 되돌린다.
            enemy.PlayIdle();
        }
    }

    private void ShuffleSpawnOrder()
    {
        for (int i = _spawnOrder.Length - 1; i > 0; i--)
        {
            int swapIndex = UnityEngine.Random.Range(0, i + 1);

            int temp = _spawnOrder[i];
            _spawnOrder[i] = _spawnOrder[swapIndex];
            _spawnOrder[swapIndex] = temp;
        }
    }

    private bool IsAlive(AutoBattleUnit enemy)
    {
        return null != enemy && enemy.gameObject.activeSelf;
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;

        if (null != _player)
        {
            _player.Stop();
        }

        if (null != _enemies)
        {
            foreach (AutoBattleUnit enemy in _enemies)
            {
                if (null == enemy)
                {
                    continue;
                }

                enemy.Stop();
            }
        }

        if (null != _background)
        {
            _background.StopScroll();
        }
    }
}
