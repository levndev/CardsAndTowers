using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingRotation : MonoBehaviour
{
    public float Speed;

    private void FixedUpdate()
    {
        transform.Rotate(transform.forward, Speed * Time.fixedDeltaTime);
    }
}
