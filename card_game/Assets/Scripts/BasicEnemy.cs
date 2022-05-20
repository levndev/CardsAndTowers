using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float Speed;

    private float movementChange;
    private new Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        var direction = movementChange > 4 ? Vector3.up : Vector3.down;
        var rotation = movementChange > 4 ? 0 : 180f;
        rigidbody.MoveRotation(rotation);
        rigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * Speed);
        movementChange += Time.fixedDeltaTime;
        if (movementChange > 8)
            movementChange = 0;
    }
}
