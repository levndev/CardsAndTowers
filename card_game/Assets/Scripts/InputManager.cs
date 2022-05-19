using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameObject UiCanvas;

    public event EventHandler<TapEventArgs> TapRegistered;

    private void OnTapRegistered(TapEventArgs e)
    {
        TapRegistered?.Invoke(this, e);
    }

    void OnTap(InputValue value)
    {
        var position = value.Get<Vector2>();
        if (IsUiElementHit(position))
            return;
        var worldPosition = Camera.main.ScreenToWorldPoint(position);
        OnTapRegistered(new TapEventArgs(worldPosition));
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

    public TapEventArgs(Vector2 position)
    {
        Position = position;
    }
}

