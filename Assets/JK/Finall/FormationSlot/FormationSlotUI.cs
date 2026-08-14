using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FormationSlotUI : MonoBehaviour
{
    [SerializeField] protected Image _formationImage;

    protected virtual void Awake()
    {
        UnityUtility.ValidateReference(_formationImage, nameof(_formationImage));
    }

    public void SetSprite(string spriteKey)
    {
        UIUtility.SetSpriteAsync(_formationImage, spriteKey).Forget();
    }
}