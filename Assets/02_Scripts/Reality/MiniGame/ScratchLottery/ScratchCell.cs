using UnityEngine;
using UnityEngine.UI;

public class ScratchCell : MonoBehaviour
{
    private const int COVER_SIZE = 64;

    [SerializeField] private Color32 _coverColor = new Color32(170, 170, 175, 255);
    [SerializeField] private Image _imgSymbol;
    [SerializeField] private Image _imgCover;
    [SerializeField, Range(0.1f, 1f)] private float _revealThreshold = 0.5f;

    private Texture2D _coverTexture;
    private Color32[] _pixels;
    private int _erasedCount;

    public bool IsRevealed { get; private set; }

    public RectTransform CoverRect
    {
        get
        {
            return _imgCover.rectTransform;
        }
    }

    public float ErasedRatio
    {
        get
        {
            return _erasedCount / (float)_pixels.Length;
        }
    }
    private void OnDestroy()
    {
        if (_coverTexture != null)
        {
            Destroy(_coverTexture);
            _coverTexture = null;
        }
    }

    public void Erase(Vector2 normalizedPosition, int brushRadius)
    {
        if (IsRevealed) return;

        int centerX = Mathf.RoundToInt(normalizedPosition.x * COVER_SIZE);
        int centerY = Mathf.RoundToInt(normalizedPosition.y * COVER_SIZE);
        int radiusSquared = brushRadius * brushRadius;
        bool isChanged = false;

        for (int offsetY = -brushRadius; offsetY <= brushRadius; offsetY++)
        {
            int y = centerY + offsetY;

            if (y < 0 || COVER_SIZE <= y)
            {
                continue;
            }

            for (int offsetX = -brushRadius; offsetX <= brushRadius; offsetX++)
            {
                int x = centerX + offsetX;
                
                if (x < 0 || COVER_SIZE <= x)
                {
                    continue;
                }

                if (radiusSquared < offsetX * offsetX + offsetY * offsetY)
                {
                    continue;
                }

                int pixelIndex = y * COVER_SIZE + x;

                if (0 == _pixels[pixelIndex].a)
                {
                    continue;
                }

                _pixels[pixelIndex].a = 0;
                _erasedCount++;
                isChanged = true;
            }
        }

        if (isChanged == false) return;

        _coverTexture.SetPixels32(_pixels);
        _coverTexture.Apply();

        TryReaveal();
    }

    public void Initialize(Sprite symbolSprite)
    {
        _imgSymbol.sprite = symbolSprite;

        IsRevealed = false;
        _erasedCount = 0;

        EnsureCoverTexture();
        FillCover();

        _imgCover.enabled = true;
        
    }

    private void TryReaveal()
    {
        if (IsRevealed) return;
        if (ErasedRatio < _revealThreshold) return;

        IsRevealed = true;
        _imgCover.enabled = false;
    }
    private void EnsureCoverTexture()
    {
        if (_coverTexture != null)
        {
            return;
        }

        _coverTexture = new Texture2D(COVER_SIZE, COVER_SIZE, TextureFormat.RGBA32, false);
        _coverTexture.filterMode = FilterMode.Bilinear;
        _pixels = new Color32[COVER_SIZE * COVER_SIZE];
        _imgCover.sprite = Sprite.Create(_coverTexture, new Rect(0f, 0f, COVER_SIZE, COVER_SIZE), new Vector2(0.5f, 0.5f));
    }

    private void FillCover()
    {
        for (int index = 0; index < _pixels.Length; index++)
        {
            _pixels[index] = _coverColor;
        }

        _coverTexture.SetPixels32(_pixels);
        _coverTexture.Apply();
    }

    
}
