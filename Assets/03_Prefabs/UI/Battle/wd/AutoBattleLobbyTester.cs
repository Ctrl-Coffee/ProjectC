using UnityEngine;
using UnityEngine.InputSystem;

public class AutoBattleLobbyTester : MonoBehaviour
{
    [SerializeField] private AutoBattleController _autoBattlePrefab;
    [SerializeField] private Key _toggleKey = Key.F9;
    [SerializeField] private bool _spawnOnStart;

    [SerializeField] private string _parentObjectName = "DreamBackground(Clone)";

    [Header("배치")]
    [SerializeField] private Vector3 _localPosition;
    [SerializeField] private float _scale = 1f;

    private AutoBattleController _instance;

    private void Start()
    {
        if (_spawnOnStart)
        {
            Spawn();
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (null != keyboard && keyboard[_toggleKey].wasPressedThisFrame)
        {
            Toggle();
        }

        ApplyTransform();
    }

    public void Toggle()
    {
        if (null == _instance)
        {
            Spawn();
            return;
        }

        Despawn();
    }

    public void Spawn()
    {
        if (null != _instance)
        {
            return;
        }

        if (null == _autoBattlePrefab)
        {
            Logger.LogError("자동전투 테스트 : 프리팹이 지정되지 않았습니다.");
            return;
        }

        _instance = Instantiate(_autoBattlePrefab, FindParent());

        ApplyTransform();
    }

    public void Despawn()
    {
        if (null == _instance)
        {
            return;
        }

        Destroy(_instance.gameObject);
        _instance = null;
    }

    private Transform FindParent()
    {
        if (string.IsNullOrEmpty(_parentObjectName))
        {
            return null;
        }

        GameObject parentObject = GameObject.Find(_parentObjectName);

        if (null == parentObject)
        {
            Logger.LogWarning($"자동전투 테스트 : {_parentObjectName} 을 찾지 못해 씬 루트에 생성합니다. 꿈속 로비에 들어간 뒤에 눌러야 합니다.");
            return null;
        }

        return parentObject.transform;
    }

    private void ApplyTransform()
    {
        if (null == _instance)
        {
            return;
        }

        _instance.transform.localPosition = _localPosition;
        _instance.transform.localScale = Vector3.one * _scale;
    }
}
