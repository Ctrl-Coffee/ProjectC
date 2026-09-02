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
    [SerializeField] private Camera _worldCamera;

    [Header("Screen Anchor")]
    [SerializeField] private float _bottomAnchorOffset;

    [Header("Drop")]
    [SerializeField] private AutoBattleDropSpawner _dropSpawner;

    [Header("Enemy Movement")]
    [SerializeField] private float _enemyApproachDistance = 3f;
    [SerializeField] private float _enemyApproachDuration = 3f;
    [SerializeField] private float _groundYOffset;
    [SerializeField] private float _formationYStep = 0.35f;
    [SerializeField] private int _formationSortingStep = 1;
    [SerializeField] private float _spawnMarginX = 2f;
    [SerializeField] private float _enemySpacing = 1.2f;

    [Header("Attack")]
    [SerializeField] private int _attackCountPerEnemy = 3;
    [SerializeField] private float _attackDelay = 0.5f;
    [SerializeField] private float _hitDelay = 0.2f;
    [SerializeField] private float _deathDelay = 0.5f;

    [Header("Loop")]
    [SerializeField] private float _nextWaveDelay = 1f;

    [Header("애니메이션 세트")]
    [SerializeField] private UnitAnimationSet _playerAnimationSet;
    [SerializeField] private UnitAnimationSet[] _enemyAnimationSetPool;

    private float _groundY;
    private int _playerSortingOrder;

    private int[] _spawnOrder;

    private UniTask[] _moveTasks;

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

        if (null == _dropSpawner)
        {
            Logger.LogWarning("자동전투 : 드랍 스포너가 지정되지 않아 재화가 떨어지지 않습니다.");
        }

        return true;
    }

    private void Initialize()
    {
        _player.Initialize();
        _player.ApplyAnimationSet(_playerAnimationSet);

        _player.PlayRun();

        AnchorToCameraBottom();

        _groundY = _player.GetGroundY() + _groundYOffset;
        _playerSortingOrder = _player.GetSortingOrder();

        _spawnOrder = new int[_enemies.Length];
        _moveTasks = new UniTask[_enemies.Length];

        for (int i = 0; i < _enemies.Length; i++)
        {
            _spawnOrder[i] = i;

            if (null == _enemies[i])
            {
                continue;
            }

            _enemies[i].Initialize();
            _enemies[i].PlayIdle();
        }
    }

    private UnitAnimationSet PickAnimationSet()
    {
        if (null == _enemyAnimationSetPool || 0 == _enemyAnimationSetPool.Length)
        {
            return null;
        }

        return _enemyAnimationSetPool[UnityEngine.Random.Range(0, _enemyAnimationSetPool.Length)];
    }

    public void SetEnemyAnimationSetPool(UnitAnimationSet[] animationSets)
    {
        _enemyAnimationSetPool = animationSets;
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
        _player.PlayIdle();

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (false == IsAlive(_enemies[i]))
            {
                continue;
            }

            await DefeatEnemy(_enemies[i], cancellationToken);
        }

        _player.PlayRun();

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
        int lineIndex = 0;

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (false == IsAlive(_enemies[i]))
            {
                _moveTasks[i] = UniTask.CompletedTask;
                continue;
            }

            Vector3 targetPosition = new Vector3(
                _player.transform.position.x + _enemyApproachDistance + lineIndex * _enemySpacing,
                _enemies[i].transform.position.y,
                _enemies[i].transform.position.z);

            _moveTasks[i] = _enemies[i].MoveTo(targetPosition, GetApproachDuration(_enemies[i].transform.position, targetPosition));

            lineIndex++;
        }

        await UniTask.WhenAll(_moveTasks);

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

        await _player.WaitForAttackHitAsync(_attackDelay, cancellationToken);

        enemy.PlayHit();

        await UniTask.Delay(Mathf.RoundToInt(_hitDelay * 1000f), cancellationToken: cancellationToken);
    }

    private async UniTask PlayDeath(AutoBattleUnit enemy, CancellationToken cancellationToken)
    {
        enemy.PlayDeath();

        float deathWaitSeconds = Mathf.Max(_deathDelay, enemy.GetDeathAnimationLength());

        await UniTask.Delay(Mathf.RoundToInt(deathWaitSeconds * 1000f), cancellationToken: cancellationToken);

        if (null != _dropSpawner)
        {
            _dropSpawner.Spawn(enemy.GetDropPosition(), _player, _playerSortingOrder + 1);
        }

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

            _enemies[i].gameObject.SetActive(false);
        }

        ShuffleSpawnOrder();

        int enemyCount = UnityEngine.Random.Range(1, _enemies.Length + 1);

        float spawnX = GetSpawnX();

        for (int i = 0; i < enemyCount; i++)
        {
            AutoBattleUnit enemy = _enemies[_spawnOrder[i]];

            if (null == enemy)
            {
                continue;
            }

            enemy.gameObject.SetActive(true);

            enemy.ApplyAnimationSet(PickAnimationSet());

            enemy.PlayIdle();
        }

        int lineIndex = 0;

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (false == IsAlive(_enemies[i]))
            {
                continue;
            }

            _enemies[i].PlaceOnGround(
                spawnX + lineIndex * _enemySpacing,
                _groundY + lineIndex * _formationYStep);

            _enemies[i].SetSortingOrder(_playerSortingOrder - 1 - lineIndex * _formationSortingStep);

            lineIndex++;
        }
    }

    private float GetApproachDuration(Vector3 fromPosition, Vector3 toPosition)
    {
        float worldSpeed = _background.GetScrollWorldSpeed();

        if (0f >= worldSpeed)
        {
            return _enemyApproachDuration;
        }

        return Mathf.Abs(toPosition.x - fromPosition.x) / worldSpeed;
    }

    private void AnchorToCameraBottom()
    {
        Camera camera = GetWorldCamera();

        if (null == camera || false == camera.orthographic)
        {
            return;
        }

        float targetGroundY = camera.transform.position.y - camera.orthographicSize + _bottomAnchorOffset;

        Vector3 position = transform.position;

        position.x = camera.transform.position.x;
        position.y += targetGroundY - _player.GetGroundY();

        transform.position = position;
    }

    private Camera GetWorldCamera()
    {
        if (null != _worldCamera)
        {
            return _worldCamera;
        }

        return Camera.main;
    }

    private float GetSpawnX()
    {
        Camera camera = GetWorldCamera();

        if (null == camera || false == camera.orthographic)
        {
            return _player.transform.position.x + _enemyApproachDistance + _spawnMarginX;
        }

        float halfWidth = camera.orthographicSize * camera.aspect;

        return camera.transform.position.x + halfWidth + _spawnMarginX;
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
