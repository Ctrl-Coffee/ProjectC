using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class StagePoint : MonoBehaviour
{
    [SerializeField] private string _stageId;

    private void Awake()
    {
        CircleCollider2D circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.isTrigger = true;
    }

    public void SelectStage()
    {
        GameManager.UI.OpenStageInfo(_stageId).Forget();
    }
}
