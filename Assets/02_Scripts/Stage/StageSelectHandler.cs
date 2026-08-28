using UnityEngine;
using UnityEngine.EventSystems;

public class StageSelectHandler : MonoBehaviour
{
    [SerializeField] private string _chapter;
    [SerializeField] private string _stage;

    public void SelectStage()
    {
        Logger.Log($"stage_{_chapter}_{_stage} 스테이지 선택");
    }

}
