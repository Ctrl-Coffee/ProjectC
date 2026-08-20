using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUI : UIBase
{
    [SerializeField] private TextMeshProUGUI _gameVersionText;

    [SerializeField] private GameObject _bgmNormal;
    [SerializeField] private GameObject _bgmMute;
    [SerializeField] private GameObject _sfxNormal;
    [SerializeField] private GameObject _sfxMute;

    [SerializeField] private UIButtonComponent _bgmButton;
    [SerializeField] private UIButtonComponent _sfxButton;
    [SerializeField] private UIButtonComponent _privacyPolicyButton;
    [SerializeField] private UIButtonComponent _closeButton;

    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private const string PRIVACY_POLICY_URL = "https://github.com/Ctrl-Coffee/ProjectC";
    private const float MIN_VALUE = 0.0001f;

    private void Awake()
    {
        SetGameVersion();
    }

    private void OnEnable()
    {
        float bgmVolume = GameManager.Sound.BGMVolume;
        float sfxVolume = GameManager.Sound.SFXVolume;

        _bgmSlider.SetValueWithoutNotify(bgmVolume);
        _sfxSlider.SetValueWithoutNotify(sfxVolume);

        IconUpdate(_bgmNormal, _bgmMute, bgmVolume);
        IconUpdate(_sfxNormal, _sfxMute, sfxVolume);

        _sfxButton.BindButtonEvent(OnToggleSFX);
        _bgmButton.BindButtonEvent(OnToggleBGM);
        _privacyPolicyButton.BindButtonEvent(OnPrivacyPolicyURL);
        _closeButton.BindButtonEvent(OnClose);
    }

    private void OnDisable()
    {
        _sfxButton.UnBindButtonAllEvent();
        _bgmButton.UnBindButtonAllEvent();
        _privacyPolicyButton.UnBindButtonAllEvent();
        _closeButton.UnBindButtonAllEvent();
    }

    public void SetBGMVolume(float volume)
    {
        GameManager.Sound.SetBGMVolume(volume);
        IconUpdate(_bgmNormal, _bgmMute, volume);
    }

    public void SetSFXVolume(float volume)
    {
        GameManager.Sound.SetSFXVolume(volume);
        IconUpdate(_sfxNormal, _sfxMute, volume);
    }

    public void OnToggleBGM()
    {
        GameManager.Sound.ToggleBGM();

        var volume = GameManager.Sound.BGMVolume;

        IconUpdate(_bgmNormal, _bgmMute, volume);

        _bgmSlider.SetValueWithoutNotify(volume);
    }

    public void OnToggleSFX()
    {
        GameManager.Sound.ToggleSFX();

        var volume = GameManager.Sound.SFXVolume;

        IconUpdate(_sfxNormal, _sfxMute, volume);

        _sfxSlider.SetValueWithoutNotify(volume);
    }

    private void OnClose()
    {
        CloseUI();
    }

    private void IconUpdate(GameObject normal, GameObject mute, float volume)
    {
        normal.SetActive(volume > MIN_VALUE);
        mute.SetActive(volume <= MIN_VALUE);
    }

    private void SetGameVersion()
    {
        _gameVersionText.text = $"Version:{Application.version}";
    }

    private void OnPrivacyPolicyURL()
    {
        Application.OpenURL(PRIVACY_POLICY_URL);
    }
}
