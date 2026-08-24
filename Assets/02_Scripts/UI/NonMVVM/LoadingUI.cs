using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIBase
{
    [Header("ETC UI")]
    [SerializeField] private TextMeshProUGUI _versionText;

    [Header("Progress")]
    [SerializeField] private TextMeshProUGUI _loadedText;
    [SerializeField] private Slider _loadingSlider;
    [SerializeField] private float _fillSpeed = 0.5f;

    private float _progress = 0f;

    private void Awake()
    {
        ResetProgress();
        SetVersionText();
    }

    private void Update()
    {
        _loadingSlider.value = Mathf.MoveTowards(_loadingSlider.value, _progress, _fillSpeed * Time.unscaledDeltaTime);
        _loadedText.SetText("Loading... {0:0.00}%", _loadingSlider.value * 100f);
    }

    public void SetProgress(float progress)
    {
        _progress = Mathf.Clamp01(progress);
    }

    public async UniTask WaitUntilFilledAsync()
    {
        while (_loadingSlider.value < _progress)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    private void ResetProgress()
    {
        _progress = 0f;
        _loadingSlider.value = 0f;
    }

    private void SetVersionText()
    {
        _versionText.text = $"Version : {Application.version}";
    }
}
