using UnityEngine;

public class AutoBattleBackground : MonoBehaviour
{
    // 매 프레임 문자열 조회를 피하려고 ID 를 미리 구해둔다.
    private static readonly int SCROLL_OFFSET_ID = Shader.PropertyToID("_ScrollOffset");

    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _scrollSpeed = 0.2f;

    // 머티리얼 사본을 만들지 않고 렌더러 단위로만 값을 덮어쓴다.
    private MaterialPropertyBlock _propertyBlock;
    private float _offset;
    private bool _isScrolling;

    public void StartScroll()
    {
        _isScrolling = true;
    }

    public void StopScroll()
    {
        _isScrolling = false;
    }

    public float GetScrollWorldSpeed()
    {
        if (null == _spriteRenderer)
        {
            return 0f;
        }

        return _scrollSpeed * _spriteRenderer.bounds.size.x;
    }

    private void Awake()
    {
        if (null == _spriteRenderer)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        _propertyBlock = new MaterialPropertyBlock();
        _isScrolling = true;

        ApplyOffset();
    }

    private void Update()
    {
        if (false == _isScrolling)
        {
            return;
        }

        _offset += _scrollSpeed * Time.deltaTime;

        _offset -= Mathf.Floor(_offset);

        ApplyOffset();
    }

    private void ApplyOffset()
    {
        if (null == _spriteRenderer)
        {
            return;
        }

        _spriteRenderer.GetPropertyBlock(_propertyBlock);

        _propertyBlock.SetFloat(SCROLL_OFFSET_ID, _offset);

        _spriteRenderer.SetPropertyBlock(_propertyBlock);
    }
}
