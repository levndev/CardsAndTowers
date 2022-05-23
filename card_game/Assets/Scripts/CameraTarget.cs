using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public InputManager InputManager;
    public float PanSpeed;
    public bool Enabled;

    private void Start()
    {
        InputManager.TouchMoved += (sender, args) => Move(args.Delta);
        Enabled = true;
    }

    private void Move(Vector2 direction)
    {
        if (Enabled)
        {
            transform.position += new Vector3(-direction.x, -direction.y, 0) * PanSpeed;
        }
    }
}
