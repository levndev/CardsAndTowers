using UnityEngine;

public class AreaProjectile : Projectile
{
    public float Radius;
    public GameObject ExplosionEffect;

    protected override void Explode(BasicEnemy hitEnemy)
    {
        var colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(Radius, Radius), 0);
        foreach (var collider in colliders)
        {
            if (collider.TryGetComponent<BasicEnemy>(out var enemy)
                && Vector3.Distance(collider.transform.position, transform.position) < Radius)
            {
                enemy.Health.TakeDamage(Damage);
            }
        }
        Instantiate(ExplosionEffect, transform.position, new Quaternion());
        Destroy(gameObject);
    }
}
