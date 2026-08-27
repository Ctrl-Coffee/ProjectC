using DG.Tweening;
using UnityEngine;

public class TestBattleHUDUI : MonoBehaviour
{
    [SerializeField] private BattleHudUI _battleHud;
    [SerializeField] private Vector3 _testWorldPosition;
    [SerializeField] private long _testDamage = 1234;

    [ContextMenu("데미지 폰트 재생")]
    private void PlayDamage()
    {
        _battleHud.ShowDamage(_testWorldPosition, _testDamage);
    }
}
