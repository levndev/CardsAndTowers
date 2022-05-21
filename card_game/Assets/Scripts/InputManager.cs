using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameObject UiCanvas;

    public event EventHandler<TapEventArgs> TapRegistered;
    public event EventHandler<TapEventArgs> TouchMoved;

    private void OnTap(InputValue value)
    {
        var position = value.Get<Vector2>();
        if (IsUiElementHit(position))
            return;
        var worldPosition = Camera.main.ScreenToWorldPoint(position);
        TapRegistered?.Invoke(this, new TapEventArgs { Position = worldPosition });
    }

    private void OnCameraMove(InputValue value)
    {
        var delta = value.Get<Vector2>();
        TouchMoved?.Invoke(this, new TapEventArgs { Delta = delta });
    }

    private bool IsUiElementHit(Vector2 touchPosition)
    {
        foreach (Transform uiElement in UiCanvas.transform)
        {
            var uiElementRectTransform = uiElement.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(uiElementRectTransform, touchPosition))
                return true;
        }
        return false;
    }
}

public class TapEventArgs : EventArgs
{
    public Vector2 Position { get; set; }
    public Vector2 Delta { get; set; }
}

