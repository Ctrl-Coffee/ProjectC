using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class GaugeSoundLoop
{
    private const string SOURCE_NAME = "GaugeSoundPlayer";
    private const string MIXER_NAME = "SoundMixer";
    private const string SFX_GROUP_NAME = "SFX";

    private AudioSource _source;

    private float _minPitch = 1f;
    private float _maxPitch = 1f;

    private bool _isPlaying;

    // 공용 SFX 소스의 pitch값을 만지면 다른 효과음까지 영향을 받음.
    // 게이지 전용 AudioSource를 따로 두고, 볼륨만 SFX 믹서 그룹에 태운다.
    public void Init(GameObject owner, float minPitch, float maxPitch)
    {
        _minPitch = minPitch;
        _maxPitch = maxPitch;

        if (null != _source || null == owner)
        {
            return;
        }

        _source = Utils.GetOrAddComponentInChild<AudioSource>(owner, SOURCE_NAME);

        _source.loop = true;
        _source.playOnAwake = false;

        AudioMixer mixer = GameManager.Resource.GetLoadedAsset<AudioMixer>(MIXER_NAME);

        if (null == mixer)
        {
            return;
        }

        AudioMixerGroup[] groups = mixer.FindMatchingGroups(SFX_GROUP_NAME);

        if (0 < groups.Length)
        {
            _source.outputAudioMixerGroup = groups[0];
        }
    }

    public void Play()
    {
        if (null == _source)
        {
            return;
        }

        _isPlaying = true;

        if (null == _source.clip)
        {
            LoadAndPlayAsync().Forget();
            return;
        }

        _source.Play();
    }

    public void Stop()
    {
        _isPlaying = false;

        if (null == _source)
        {
            return;
        }

        _source.Stop();
    }

    public void SetProgress(float progress)
    {
        if (null == _source)
        {
            return;
        }

        _source.pitch = Mathf.Lerp(_minPitch, _maxPitch, Mathf.Clamp01(progress));
    }

    private async UniTaskVoid LoadAndPlayAsync()
    {
        AudioClip clip = await GameManager.Resource.LoadAssetAsync<AudioClip>(AddressablePath.Audio.GAUGE_MOVE);

        if (null == clip)
        {
            Logger.LogError($"{AddressablePath.Audio.GAUGE_MOVE}를 찾을 수 없습니다! 어드레서블 설정을 확인해주세요.");
            return;
        }

        // 로드가 끝나기 전에 게임이 끝났으면 재생 X
        if (null == _source || !_isPlaying)
        {
            return;
        }

        _source.clip = clip;
        _source.Play();
    }
}
