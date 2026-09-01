using UnityEngine;

public class InteractObjectHandler : MonoBehaviour
{
    private BackgroundInputHandler _inputHandler;

    private void Awake()
    {
        _inputHandler = GetComponent<BackgroundInputHandler>();
    }

    private void OnEnable()
    {
        _inputHandler.OnTapped += OnInteract;
    }

    private void OnDisable()
    {
        _inputHandler.OnTapped -= OnInteract;
    }

    private void OnInteract(Vector2 screenPosition)
    {
        Vector3 worldPosition = _inputHandler.GetWorldPosition(screenPosition);
        Collider2D targetCollider = Physics2D.OverlapPoint(worldPosition);

        if (targetCollider == null)
            return;

        var hudView = GameManager.UI.GetUI<RealHudView>();

        if (targetCollider.CompareTag("Bed"))
        {
            hudView.OnChangeSceenToDream();
        }
        else if(targetCollider.CompareTag("Computer"))
        {
            hudView.OnOpenWorkInfoUI();
        }
        else if(targetCollider.CompareTag("CoffeePot"))
        {
            hudView.OnCoffeePot();
        }
    }
}
