using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtility
{
    public static async UniTask SetSpriteAsync(Image image, string spriteKey)
    {
        if (image == null)
        {
            Debug.LogError($"전달된 이미지가 Null 입니다.");
            return;
        }

        if (string.IsNullOrWhiteSpace(spriteKey))
        {
            image.sprite = null;
            return;
        }

        Sprite sprite = await ResourceUtility.LoadSpriteAsync(spriteKey);
        image.sprite = sprite;
    }
}