using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager
{
    public float BGMVolume => _isBGMMuted ? 0f : _bgmVolume;
    public float SFXVolume => _isSFXMuted ? 0f : _sfxVolume;

    private AudioMixer _mixer;

    private AudioSource _bgmPlayer;
    private AudioSource _sfxPlayer;

    private const float _mutedDecibel = -80f;

    // TODO: 추후 정장된 값 로드
    private float _bgmVolume = 0.2f;
    private float _sfxVolume = 0.2f;

    // TODO: 추후 정장된 값 로드
    private bool _isBGMMuted = false;
    private bool _isSFXMuted = false;

    public void Init(GameObject gameManager)
    {
        _mixer = GameManager.Resource.GetLoadedAsset<AudioMixer>("SoundMixer");

        _bgmPlayer = Utils.GetOrAddComponentInChild<AudioSource>(gameManager, "BGMSourcePlayer");
        _sfxPlayer = Utils.GetOrAddComponentInChild<AudioSource>(gameManager, "SFXSourcePlayer");

        _bgmPlayer.outputAudioMixerGroup = _mixer.FindMatchingGroups("BGM")[0];
        _sfxPlayer.outputAudioMixerGroup = _mixer.FindMatchingGroups("SFX")[0];

        ApplyVolume("BGMVolume", _bgmVolume, false);
        ApplyVolume("SFXVolume", _sfxVolume, false);
    }

    public void PlayBGM(string soundPath)
    {
        LoadAndPlayAudioClip(_bgmPlayer, soundPath, isLoop: true).Forget();
    }

    public void PlaySFX(string soundPath)
    {
        LoadAndPlayAudioClip(_sfxPlayer, soundPath).Forget();
    }

    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }

    public void StopSFX()
    {
        _sfxPlayer.Stop();
    }

    public void SetBGMVolume(float volume)
    {
        _bgmVolume = volume;
        _isBGMMuted = false;

        ApplyVolume("BGMVolume", _bgmVolume, false);
    }

    public void SetSFXVolume(float volume)
    {
        _sfxVolume = volume;
        _isSFXMuted = false;

        ApplyVolume("SFXVolume", _sfxVolume, false);
    }

    public void PauseBGM()
    {
        _bgmPlayer.Pause();
    }

    public void ResumeBGM()
    {
        _bgmPlayer.UnPause();
    }

    public void ToggleBGM()
    {
        _isBGMMuted = !_isBGMMuted;

        ApplyVolume("BGMVolume", _bgmVolume, _isBGMMuted);
    }

    public void ToggleSFX()
    {
        _isSFXMuted = !_isSFXMuted;

        ApplyVolume("SFXVolume", _sfxVolume, _isSFXMuted);
    }

    private void ApplyVolume(string parameterName, float volume, bool isMuted)
    {
        float decibel = isMuted || volume <= 0f ? _mutedDecibel : Mathf.Max(_mutedDecibel, 10f * Mathf.Log(volume, 2f));

        _mixer.SetFloat(parameterName, decibel);
    }

    private async UniTaskVoid LoadAndPlayAudioClip(AudioSource audioSource, string audioPath, bool isLoop = false)
    {
        AudioClip clip = await GameManager.Resource.LoadAssetAsync<AudioClip>(audioPath);
        if (clip == null)
        {
            Debug.LogError($"{audioPath}를 찾을 수 없습니다! 어드레서블 설정이 되어 있는지 확인해주세요.");
            return;
        }

        if (isLoop == true)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
