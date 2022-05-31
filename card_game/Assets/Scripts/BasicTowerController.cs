using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class BasicTowerController : MonoBehaviour
{
    public double HealthPoints;
    public double FireRate;
    public float RotationSpeed;
    public float PrefireCoefficient;
    public Projectile Projectile;

    private GameObject turret;
    private List<BasicEnemy> enemiesInRange = new List<BasicEnemy>();
    private BasicEnemy target;
    private double fireCooldown;
    private Transform bulletSpawnPoint;
    private float fireAngleThreshold = 10;

    private void Awake()
    {
        turret = transform.Find("Turret").gameObject;
        bulletSpawnPoint = turret.transform.Find("BulletSpawnPoint");
    }

    void FixedUpdate()
    {
        if (target == null)
        {
            target = FindClosestEnemy();
            if (target == null)
                return;
        }
        var distance = Vector2.Distance(turret.transform.position, target.transform.position);
        var prefireVector = PrefireCoefficient * distance * target.Movement / Projectile.Speed;
        var fireTarget = (Vector2)target.transform.position + prefireVector;
        RotateTo(fireTarget, out var canFire);
        if (canFire)
        {
            if (fireCooldown >= 1/FireRate)
            {
                LaunchProjectile();
                fireCooldown = 0;
            }
            else
                fireCooldown += Time.fixedDeltaTime;
        }
    }

    private void RotateTo(Vector2 fireTarget, out bool canFire)
    {
        var directionToTarget = fireTarget - (Vector2)turret.transform.position;
        var angleToTarget = Vector2.SignedAngle(turret.transform.up, directionToTarget);
        canFire = Math.Abs(angleToTarget) < fireAngleThreshold;
        float rotationAngle;
        if (Math.Abs(angleToTarget) < RotationSpeed)
            rotationAngle = angleToTarget;
        else
            rotationAngle = angleToTarget > 0 ? RotationSpeed : -RotationSpeed;
        turret.transform.RotateAround(transform.position, Vector3.forward, rotationAngle);
    }

    private void LaunchProjectile()
    {
        Instantiate(Projectile.transform.gameObject, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
    }

    private BasicEnemy FindClosestEnemy()
    {
        if (enemiesInRange.Count == 0)
            return null;
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
        return closestEnemy;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
        {
            enemiesInRange.Add(enemy);
            enemy.OnTowerRangeEnter(this);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
            enemiesInRange.Remove(enemy);
    }
}
