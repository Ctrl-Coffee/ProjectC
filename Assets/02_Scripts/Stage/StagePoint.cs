using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StagePoint : MonoBehaviour
{
    private int _chapter;
    private int _stage;

    private void Awake()
    {
        _chapter = transform.parent.GetSiblingIndex() + 1;
        _stage = transform.GetSiblingIndex() + 1;
        GetComponent<Collider2D>().isTrigger = true;
    }

    public void SelectStage()
    {
        Logger.Log($"{_chapter} 챕터, {_stage} 스테이지 선택!");
    }
}
