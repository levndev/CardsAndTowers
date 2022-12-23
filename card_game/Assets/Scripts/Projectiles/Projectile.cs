using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Speed;
    public double Damage;
    public double Lifetime;

    private double timeElapsed;
    private new Rigidbody2D rigidbody;

    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(transform.position + transform.up * Time.fixedDeltaTime * Speed);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
        {
            Explode(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BasicEnemy>(out var enemy))
        {
            Explode(enemy);
        }
    }

    protected virtual void Explode(BasicEnemy enemy)
    {
        if (enemy != null)
        {
            enemy.Health.TakeDamage(Damage);
        }
        Destroy(transform.gameObject);
    }
}
