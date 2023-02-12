using UnityEngine;

public class SafeArea : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    
    RectTransform _panelSafeArea;
    private Rect _currentSafeArea = new Rect();
    private ScreenOrientation _currentOrientation = ScreenOrientation.Portrait;

    private void Start()
    {
        _currentOrientation = ScreenOrientation.Portrait;
        _currentSafeArea = Screen.safeArea;
    }

    void ApplySafeArea()
    {
        if(_panelSafeArea == null)
            return;
        Rect safearea = Screen.safeArea;
        Vector2 anchorMin = safearea.position;
        Vector2 anchorMax = safearea.position - safearea.size;

        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
         
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;

        _panelSafeArea.anchorMin = anchorMin;
        _panelSafeArea.anchorMax = anchorMax;

        _currentOrientation = Screen.orientation;
        _currentSafeArea = Screen.safeArea;
    }

    private void Update()
    {
        if (_currentOrientation != Screen.orientation || (_currentSafeArea != Screen.safeArea))
        {
            ApplySafeArea();
        }
    }
}