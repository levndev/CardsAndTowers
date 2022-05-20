using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class BasicTowerController : MonoBehaviour
{
    public double AttackDamage;
    public double HealthPoints;
    public double FireRate;
    public Projectile Projectile;
    public float PrefireCoefficient;

    private GameObject turret;
    public List<BasicEnemy> enemiesInRange = new List<BasicEnemy>();
    private BasicEnemy target;
    private double fireCooldown;
    private Transform bulletSpawnPoint;

    private void Awake()
    {
        turret = transform.Find("Turret").gameObject;
        bulletSpawnPoint = turret.transform.Find("BulletSpawnPoint");
    }

    void FixedUpdate()
    {
        float distance;
        (target, distance) = FindClosestEnemy();
        if (target != null)
        {
            var prefireDistance = PrefireCoefficient * target.Speed * distance / Projectile.Speed;
            RotateTo(target.transform.position + target.transform.up * prefireDistance);
            if (fireCooldown >= FireRate)
            {
                LaunchProjectile();
                fireCooldown = 0;
            }
            else
                fireCooldown += Time.fixedDeltaTime;
        }
    }

    private void RotateTo(Vector3 target)
    {
        var offset = 90f;
        var direction = target - transform.position;
        direction.Normalize();
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * (angle - offset));
    }

    private void LaunchProjectile()
    {
        Instantiate(Projectile.transform.gameObject, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private (BasicEnemy Enemy, float Distance) FindClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
            return (null, 0);
        var minDistance = float.MaxValue;
        BasicEnemy closestEnemy = null;
        foreach (var enemy in enemiesInRange)
        {
            var distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }
        return (closestEnemy, minDistance);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
            enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
            enemiesInRange.Remove(enemy);
    }
}
