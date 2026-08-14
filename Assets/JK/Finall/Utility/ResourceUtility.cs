using Cysharp.Threading.Tasks;
using UnityEngine;

public static class ResourceUtility
{
    public static async UniTask<Sprite> LoadSpriteAsync(string key)
    {
        Sprite sprite = GameManager.Resource.GetLoadedAsset<Sprite>(key);

        if (sprite == null)
        {
            sprite = await GameManager.Resource.LoadAssetAsync<Sprite>(key);
        }

        return sprite;
    }
}
