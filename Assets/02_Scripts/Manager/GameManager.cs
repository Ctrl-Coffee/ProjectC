using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    public static ResourceManager Resource { get { return Instance._resourceManager; } }
    public static NetworkManager Network { get { return Instance._networkManager; } }
    public static DataTableManager DataTable { get { return Instance._dataTable; } }
    public static PoolManager Pool { get { return Instance._poolManager; } }
    public static TimeManager Time { get { return Instance._timeManager; } }
    public static UIManager UI { get { return Instance._uiManager; } }
    public static PerkManager Perk { get { return Instance._perkManager; } }
    public static SoundManager Sound { get { return Instance._soundManager; } }
    public static BattleManager Battle { get { return Instance._battleManager; } }
    public static StageManager Stage { get { return Instance._stageManager; } }

    public static GameSession Session { get { return Instance._gameSession; } }
    public static ViewModelFactory ViewModel { get { return Instance._viewModelFactory; } }

    #region Manager Variables

    private ResourceManager _resourceManager = new();
    private NetworkManager _networkManager = new();
    private DataTableManager _dataTable = new();
    private PoolManager _poolManager = new();
    private TimeManager _timeManager = new();
    private UIManager _uiManager = new();
    private SoundManager _soundManager = new();
    private PerkManager _perkManager = new();
    private BattleManager _battleManager = new();
    private StageManager _stageManager = new();

    private GameSession _gameSession;
    private ViewModelFactory _viewModelFactory;
    private string _currentLobbyLabel;
    private bool _isChangingLobby;
    #endregion

    private void Update()
    {
        Time.OnUpdate();
        Battle.OnUpdate();
    }

    #region Init

    protected override void Init()
    {
        base.Init();

        _dataTable.LoadAllData();

        InitializeLoginAsync().Forget();
    }

    private async UniTask InitializeLoginAsync()
    {
        await _resourceManager.LoadContentAsync(AddressablePath.Label.UIROOT);
        await _resourceManager.LoadContentAsync(AddressablePath.Label.LOGIN);
        _uiManager.Init();

        _soundManager.Init(gameObject);
        _uiManager.OpenLoginUI();
    }

    public async UniTask InitializeAfterLoginAsync(Action<float> onProgress)
    {
        onProgress?.Invoke(0f);

        onProgress?.Invoke(0.15f);

        await _resourceManager.LoadContentAsync(AddressablePath.Label.COMMON,
            progress => onProgress?.Invoke(0.15f + progress * 0.325f));
        await _resourceManager.LoadContentAsync(AddressablePath.Label.REALITY,
            progress => onProgress?.Invoke(0.475f + progress * 0.325f));

        GameSession gameSession = new(_networkManager);
        await gameSession.LoadAllData();
        _gameSession = gameSession;

        _stageManager.Initialize();
        await _stageManager.LoadDataAsync();

        await _perkManager.LoadDataAsync();
        await AutoWorkQueue.RestoreSlots();

        _viewModelFactory = new(_gameSession, _dataTable);

        onProgress?.Invoke(0.85f);

        Transform poolRoot = Utils.CreateEmptyGameObject("PoolRoot",transform).transform;
        await _poolManager.InitAsync(poolRoot);

        await _battleManager.Initialize();

        AwayReportFlow.OnRelaunch();

        AutoWorkQueue.RunCollectLoopAsync(destroyCancellationToken).Forget();
        EnergyRecovery.RunRecoverLoopAsync(destroyCancellationToken).Forget();

        _currentLobbyLabel = AddressablePath.Label.REALITY;
        OpenLobby(_currentLobbyLabel);
        onProgress?.Invoke(1f);
    }                                                                     
    #endregion                                                            

    public void EnterReal()
    {
        ChangeLobbyAsync(AddressablePath.Label.REALITY).Forget();
    }

    public UniTask ExitReal()
    {
        return ReleaseLobbyAsync(AddressablePath.Label.REALITY);
    }

    public void EnterDream()
    {
        ChangeLobbyAsync(AddressablePath.Label.DREAM).Forget();
    }

    public UniTask ExitDream()
    {
        Battle.ReleaseBattleRoot();
        return ReleaseLobbyAsync(AddressablePath.Label.DREAM);
    }

    private async UniTask ChangeLobbyAsync(string label)
    {
        if (_isChangingLobby)
        {
            return;
        }

        // 전투에서 꿈 로비로 복귀할 때는 이미 로드한 Dream을 유지한다.
        if (_currentLobbyLabel == label)
        {
            OpenLobby(label);
            return;
        }

        _isChangingLobby = true;
        string previousLabel = _currentLobbyLabel;
        LoadingUI loadingUI = null;

        try
        {
            loadingUI = UI.OpenLoading();

            if (previousLabel == AddressablePath.Label.REALITY)
            {
                await ExitReal();
            }
            else if (previousLabel == AddressablePath.Label.DREAM)
            {
                await ExitDream();
            }

            await Resource.LoadContentAsync(label, progress => loadingUI.SetProgress(progress * 0.9f));
            OpenLobby(label);
            _currentLobbyLabel = label;
        }
        catch (Exception exception)
        {
            Logger.LogError($"로비 전환 실패 ({previousLabel} -> {label})\n{exception}");

            // 부분적으로 열린 대상 화면도 정리한 뒤 이전 로비를 복원한다.
            if (label == AddressablePath.Label.DREAM)
            {
                await ExitDream();
            }
            else
            {
                await ExitReal();
            }

            if (previousLabel != null)
            {
                await Resource.LoadContentAsync(previousLabel);
                OpenLobby(previousLabel);
                _currentLobbyLabel = previousLabel;
            }
        }
        finally
        {
            if (loadingUI != null)
            {
                loadingUI.CloseUI(true);
            }

            _isChangingLobby = false;
        }
    }

    private async UniTask ReleaseLobbyAsync(string label)
    {
        Sound.StopBGM();
        Sound.StopSFX();
        UI.ReleaseContentUI(label);

        // Destroy와 View.OnDestroy의 배경 정리가 끝난 후 핸들을 해제한다.
        await UniTask.NextFrame();
        Resource.ReleaseContent(label);
        _currentLobbyLabel = null;
    }

    private void OpenLobby(string label)
    {
        if (label == AddressablePath.Label.REALITY)
        {
            UI.OpenRealHud();
            Sound.PlayBGM(AddressablePath.Audio.BGM_LOBBY);
            return;
        }

        int chapter = Stage.HighestUnlockedChapter;
        UI.OpenDreamHud(chapter);
        Sound.PlayBGM(AddressablePath.GetChapterAudioPath(chapter));
    }

    public void RequestQuit()
    {
        QuitAfterSaveAsync().Forget();
    }

    private async UniTask QuitAfterSaveAsync()
    {
        await SaveUtil.SaveAllDataAsync();
        Application.Quit();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus == false || _gameSession == null)
        {
            return;
        }

        SaveUtil.SaveAllDataAsync().Forget();
        AwayReportFlow.SetAppActive(!pauseStatus);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        AwayReportFlow.SetAppActive(hasFocus);
    }
}
