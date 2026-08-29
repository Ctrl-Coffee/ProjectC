using DG.Tweening;
using UnityEngine;

public class HeroLevelUpEffect : MonoBehaviour
{
    [Header("펀치")]
    [SerializeField] private Transform _punchTarget;
    [SerializeField] private float _punchScale = 0.3f;
    [SerializeField] private float _punchDuration = 0.4f;

    [Header("스텟 롤업")]
    [SerializeField] private float _rollupDuration = 0.35f;

    [Header("사운드")]
    [SerializeField] private string _sfxPath = AddressablePath.Audio.STAMP_SUCCESS;

    private HeroInfoView _view;

    private HeroStatSum _from;
    private HeroStatSum _to;
    private float _fromCombatPower;
    private float _toCombatPower;

    private HeroStatSum _currentStat;
    private float _currentCombatPower;

    private Tween _rollupTween;

    public bool IsPlaying
    {
        get { return null != _rollupTween; }
    }

    public HeroStatSum CurrentStat
    {
        get { return _currentStat; }
    }

    public float CurrentCombatPower
    {
        get { return _currentCombatPower; }
    }

    public void Play(HeroInfoView view, HeroStatSum from, HeroStatSum to, float fromCombatPower, float toCombatPower)
    {
        if (null == view)
        {
            Logger.LogError("레벨업 연출 대상이 없습니다.");
            return;
        }

        Stop(false);

        _view = view;
        _from = from;
        _to = to;
        _fromCombatPower = fromCombatPower;
        _toCombatPower = toCombatPower;

        PlayPunch();
        PlaySfx();
        StartRollup();
    }

    public void Stop()
    {
        Stop(true);
    }

    /// <summary>
    /// 롤업 도중 멈추면 표기가 중간값에서 멈춤, 기본적으로 최종값을 확정하고 끝낸다.
    /// </summary>
    public void Stop(bool completeImmediately)
    {
        bool wasRolling = IsPlaying;

        if (null != _rollupTween)
        {
            _rollupTween.Kill();
            _rollupTween = null;
        }

        if (null != _punchTarget)
        {
            _punchTarget.DOKill();
            _punchTarget.localScale = Vector3.one;
        }

        HeroInfoView view = _view;

        _view = null;

        if (completeImmediately == false) return;
        if (wasRolling == false) return;
        if (null == view) return;

        view.OnStatRollupCompleted();
    }

    private void OnDisable()
    {
        Stop();
    }

    private void PlayPunch()
    {
        if (null == _punchTarget)
        {
            return;
        }

        _punchTarget.DOKill();
        _punchTarget.localScale = Vector3.one;

        _punchTarget.DOPunchScale(Vector3.one * _punchScale, _punchDuration).SetUpdate(true);
    }

    private void PlaySfx()
    {
        if (string.IsNullOrEmpty(_sfxPath))
        {
            return;
        }

        GameManager.Sound.PlaySFX(_sfxPath);
    }

    private void StartRollup()
    {
        _rollupTween = DOVirtual.Float(0f, 1f, _rollupDuration, SetRollupProgress).SetUpdate(true);
        _rollupTween.OnComplete(OnRollupCompleted);

        SetRollupProgress(0f);
    }

    private void SetRollupProgress(float progress)
    {
        if (null == _view)
        {
            return;
        }

        HeroStatSum stat = new HeroStatSum();

        stat.Attack = Mathf.Lerp(_from.Attack, _to.Attack, progress);
        stat.Hp = Mathf.Lerp(_from.Hp, _to.Hp, progress);
        stat.Defense = Mathf.Lerp(_from.Defense, _to.Defense, progress);
        stat.CriticalChance = Mathf.Lerp(_from.CriticalChance, _to.CriticalChance, progress);
        stat.BasicAttackHaste = Mathf.Lerp(_from.BasicAttackHaste, _to.BasicAttackHaste, progress);
        stat.BasicActiveSkillHaste = Mathf.Lerp(_from.BasicActiveSkillHaste, _to.BasicActiveSkillHaste, progress);

        float combatPower = Mathf.Lerp(_fromCombatPower, _toCombatPower, progress);

        _currentStat = stat;
        _currentCombatPower = combatPower;

        _view.OnStatRollupProgress(stat, combatPower);
    }

    private void OnRollupCompleted()
    {
        _rollupTween = null;

        HeroInfoView view = _view;

        _view = null;

        if (null == view) return;

        view.OnStatRollupCompleted();
    }
}
