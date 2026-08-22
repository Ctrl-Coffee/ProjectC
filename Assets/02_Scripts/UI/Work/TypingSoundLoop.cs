using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class TypingSoundLoop
{
    private const int MIN_KEY_COUNT = 4;
    private const int MAX_KEY_COUNT = 10;

    private const float MIN_KEY_INTERVAL = 0.08f;
    private const float MAX_KEY_INTERVAL = 0.18f;

    private const float MIN_LOOP_INTERVAL = 0.7f;
    private const float MAX_LOOP_INTERVAL = 1.5f;

    private static readonly string[] SOUND_PATHS =
    {
        AddressablePath.Audio.TYPING_1,
        AddressablePath.Audio.TYPING_2,
        AddressablePath.Audio.TYPING_3,
        AddressablePath.Audio.TYPING_4,
    };

    private CancellationTokenSource _cancelToken;

    private int[] _bag = new int[SOUND_PATHS.Length];
    private int _bagCursor;
    private int _lastIndex = -1;

    public void Play()
    {
        Stop();

        _bagCursor = _bag.Length;
        _lastIndex = -1;

        _cancelToken = new CancellationTokenSource();

        RunAsync(_cancelToken.Token).Forget();
    }

    public void Stop()
    {
        if (null == _cancelToken)
        {
            return;
        }

        _cancelToken.Cancel();
        _cancelToken.Dispose();
        _cancelToken = null;
    }

    private async UniTaskVoid RunAsync(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                int keyCount = UnityEngine.Random.Range(MIN_KEY_COUNT, MAX_KEY_COUNT + 1);

                for (int i = 0; i < keyCount; i++)
                {
                    GameManager.Sound.PlaySFX(SOUND_PATHS[NextSoundIndex()]);

                    float keyInterval = UnityEngine.Random.Range(MIN_KEY_INTERVAL, MAX_KEY_INTERVAL);

                    await UniTask.Delay(TimeSpan.FromSeconds(keyInterval), ignoreTimeScale: true, cancellationToken: token);
                }

                float loopInterval = UnityEngine.Random.Range(MIN_LOOP_INTERVAL, MAX_LOOP_INTERVAL);

                await UniTask.Delay(TimeSpan.FromSeconds(loopInterval), ignoreTimeScale: true, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    private int NextSoundIndex()
    {
        if (_bagCursor >= _bag.Length)
        {
            Shuffle();
            _bagCursor = 0;
        }

        int picked = _bag[_bagCursor];

        _bagCursor++;
        _lastIndex = picked;

        return picked;
    }

    private void Shuffle()
    {
        for (int i = 0; i < _bag.Length; i++)
        {
            _bag[i] = i;
        }

        for (int i = _bag.Length - 1; i > 0; i--)
        {
            int pick = UnityEngine.Random.Range(0, i + 1);

            int temp = _bag[i];
            _bag[i] = _bag[pick];
            _bag[pick] = temp;
        }

        if (1 < _bag.Length && _bag[0] == _lastIndex)
        {
            int temp = _bag[0];
            _bag[0] = _bag[_bag.Length - 1];
            _bag[_bag.Length - 1] = temp;
        }
    }
}
