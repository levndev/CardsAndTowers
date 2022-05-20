using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public float Damage;
    public double Lifetime;

    private double timeElapsed;

    private void FixedUpdate()
    {
        transform.position += transform.up * Time.fixedDeltaTime * Speed;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
            Destroy(transform.gameObject);
    }
}
