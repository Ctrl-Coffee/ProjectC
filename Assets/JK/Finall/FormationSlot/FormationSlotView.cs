using UnityEngine;
using UnityEngine.UI;

public class FormationSlotView : MonoBehaviour
{
    [SerializeField] protected Image _formationImage;

    protected virtual void Awake()
    {
        UnityUtility.ValidateReference(_formationImage, nameof(_formationImage));
    }

    public void SetSprite(Sprite sprite)
    {
        _formationImage.sprite = sprite;
    }
}