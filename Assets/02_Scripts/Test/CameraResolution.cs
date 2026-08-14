using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private SpriteRenderer _background;

    private int _lastWidth;
    private int _lastHeight;

    private void Start()
    {
        UpdateCameraSize();
    }

    private void Update()
    {
        if (_lastWidth == Screen.width && _lastHeight == Screen.height)
            return;

        UpdateCameraSize();
    }

    private void UpdateCameraSize()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        float aspect = (float)Screen.width / Screen.height;
        float worldWidth = _background.bounds.size.x;

        _camera.orthographicSize = worldWidth / (2f * aspect);
    }
}
