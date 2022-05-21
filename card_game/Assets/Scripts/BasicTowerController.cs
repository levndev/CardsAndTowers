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
    public float FireAngleThreshold = 10;
    public float PrefireCoefficient;
    public Projectile Projectile;

    private GameObject turret;
    private List<BasicEnemy> enemiesInRange = new List<BasicEnemy>();
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
        if (target == null)
            return;
        var prefireDistance = PrefireCoefficient * distance * target.CurrentSpeed / Projectile.Speed;
        RotateTo(target.transform.position + target.transform.up * prefireDistance, out var canFire);
        if (canFire)
        {
            if (fireCooldown >= FireRate)
            {
                LaunchProjectile();
                fireCooldown = 0;
            }
            else
                fireCooldown += Time.fixedDeltaTime;
        }
    }

    private void RotateTo(Vector3 target, out bool canFire)
    {
        var directionToTarget = target - transform.position;
        var angleToTarget = Vector3.SignedAngle(transform.up, directionToTarget, Vector3.forward);
        canFire = Math.Abs(angleToTarget) < FireAngleThreshold;
        float rotationAngle;
        if (Math.Abs(angleToTarget) < RotationSpeed)
            rotationAngle = angleToTarget;
        else
            rotationAngle = angleToTarget > 0 ? RotationSpeed : -RotationSpeed;
        transform.Rotate(Vector3.forward, rotationAngle);
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
