using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTarget : MonoBehaviour
{
    public InputManager InputManager;
    public float PanSpeed;

    private void Start()
    {
        InputManager.TouchMoved += (sender, args) => Move(args.Delta);
    }

    private void Move(Vector2 direction)
    {
        transform.position += new Vector3(-direction.x, -direction.y, 0) * PanSpeed;
    }
}
