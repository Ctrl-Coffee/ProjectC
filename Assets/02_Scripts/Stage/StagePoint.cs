using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class StagePoint : MonoBehaviour
{
    [SerializeField] private string _stageId;
    [SerializeField] private GameObject _lockGameObject;

    private void Awake()
    {
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
    }

    private void OnEnable()
    {
        GameManager.Stage.OnStageProgressChanged += RefreshStageState;

        RefreshStageState();
    }

    private void OnDisable()
    {
        GameManager.Stage.OnStageProgressChanged -= RefreshStageState;
    }


    public void SelectStage()
    {
        GameManager.UI.OpenStageInfo(_stageId);
    }

    private void RefreshStageState()
    {
        StageState state = GameManager.Stage.GetStageState(_stageId);

        _lockGameObject.SetActive(state == StageState.Locked);
    }
}
