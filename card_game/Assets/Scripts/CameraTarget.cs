using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public InputManager InputManager;
    public float PanSpeed;
    public bool Enabled;
    public Collider2D MapBoundary;

    private void Start()
    {
        InputManager.Touched += (args) => Move(args.Delta);
        Enabled = true;
    }

    private void Move(Vector2 direction)
    {
        if (Enabled)
        {
            var newPosition = transform.position + new Vector3(-direction.x, -direction.y, 0) * PanSpeed;
            var bounds = MapBoundary.bounds;
            bounds.center = new Vector3(bounds.center.x, bounds.center.y, newPosition.z);
            if (bounds.Contains(newPosition))
                transform.position = newPosition;
            else
                transform.position = bounds.ClosestPoint(newPosition);
        }
    }
}
