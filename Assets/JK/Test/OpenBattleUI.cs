using UnityEngine;
using UnityEngine.UI;

public class OpenBattleUI : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _battleUI;

    private void Awake()
    {
        _battleUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OpenBattle);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OpenBattle()
    {
        _battleUI.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
