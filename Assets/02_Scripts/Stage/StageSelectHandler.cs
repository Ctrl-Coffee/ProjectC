using UnityEngine;

public class StageSelectHandler : MonoBehaviour
{
    private BackgroundInputHandler _inputHandler;

    private void Awake()
    {
        _inputHandler = GetComponent<BackgroundInputHandler>();
    }

    private void OnEnable()
    {
        _inputHandler.OnTapped += SelectStage;
    }

    private void OnDisable()
    {
        _inputHandler.OnTapped -= SelectStage;
    }

    private void SelectStage(Vector2 screenPosition)
    {
        Vector3 worldPosition = _inputHandler.GetWorldPosition(screenPosition);
        Collider2D targetCollider = Physics2D.OverlapPoint(worldPosition);

        if (targetCollider == null)
            return;

        if (targetCollider.TryGetComponent(out StagePoint stagePoint))
        {
            stagePoint.SelectStage();
        }
    }
}
