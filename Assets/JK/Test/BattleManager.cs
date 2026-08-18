using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField] private BattleField _battleField;

    private void Awake()
    {
        Instance = this;

        if (_battleField == null)
        {
            SpawnBattleField().Forget();
        }
    }

    private async UniTask SpawnBattleField()
    {
        GameObject prefab = await Addressables.LoadAssetAsync<GameObject>("Prefabs/BattleField");

        GameObject instance = Instantiate(prefab);
        _battleField = instance.GetComponent<BattleField>();
    }

    public void StartBattle(string mainId, IReadOnlyList<string> companionIds, IReadOnlyList<string> enemyIds)
    {
        _battleField.InitializeField(mainId, companionIds, enemyIds);
    }
}
