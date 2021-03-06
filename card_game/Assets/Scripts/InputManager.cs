using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class InputManager : MonoBehaviour
{
    public GameObject UiCanvas;

    public event Action<TapEventArgs> Touched;

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        if (Touch.activeTouches.Count > 0)
        {
            var activeTouch = Touch.activeTouches[0];
            if (!IsValid(activeTouch) || IsUiElementHit(activeTouch.screenPosition))
                return;
            Touched?.Invoke(new TapEventArgs
            {
                Position = Camera.main.ScreenToWorldPoint(activeTouch.screenPosition),
                StartPosition = Camera.main.ScreenToWorldPoint(activeTouch.startScreenPosition),
                Delta = activeTouch.delta
            });
        }
    }

    private bool IsUiElementHit(Vector2 touchPosition)
    {
        foreach (Transform uiElement in UiCanvas.transform)
        {
            var uiElementRectTransform = uiElement.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(uiElementRectTransform, touchPosition) && uiElement.gameObject.activeInHierarchy)
                return true;
        }
        return false;
    }

    private bool IsValid(Touch touch)
        => touch.valid && float.IsFinite(touch.screenPosition.x) && float.IsFinite(touch.screenPosition.y);
}

public class TapEventArgs : EventArgs
{
    public Vector2 Position { get; set; }
    public Vector2 StartPosition { get; set; }
    public Vector2 Delta { get; set; }
}

