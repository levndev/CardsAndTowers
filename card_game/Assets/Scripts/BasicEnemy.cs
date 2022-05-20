using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float Speed;

    private float movementChange;

    private void FixedUpdate()
    {
        var direction = movementChange > 4 ? Vector3.up : Vector3.down;
        var rotation = movementChange > 4 ? Vector3.zero : Vector3.forward * 180f;
        transform.rotation = Quaternion.Euler(rotation);
        transform.position += direction * Time.fixedDeltaTime * Speed;
        movementChange += Time.fixedDeltaTime;
        if (movementChange > 8)
            movementChange = 0;
    }
}
