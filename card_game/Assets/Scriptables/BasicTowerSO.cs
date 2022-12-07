using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "CardsAndTowers/Basic Tower")]
public class BasicTowerSO : TowerSO
{
    private class BasicTowerState : TowerState
    {
        public GameObject Turret;
        public GameObject Base;
        public BasicEnemy Target;
        public double FireCooldown;
        public Transform BulletSpawnPoint;
        public EntityTracker Tracker;
    }
    [SerializeField] private double fireRate;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float prefireCoefficient;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float fireAngleThreshold;
    [SerializeField] private float rangeRadius;
    [SerializeField] private Sprite TurretSprite;
    [SerializeField] private GameObject shootEffect;
    [SerializeField] private GameObject deathEffect;

    public override void Init(TowerController sender)
    {
        base.Init(sender);
        var state = new BasicTowerState()
        {
            Turret = sender.transform.Find("Turret").gameObject,
            Base = sender.transform.Find("Base").gameObject,
        };
        state.BulletSpawnPoint = state.Turret.transform.Find("BulletSpawnPoint");
        var collider = sender.gameObject.AddComponent<CircleCollider2D>();
        collider.radius = rangeRadius;
        collider.isTrigger = true;
        state.Tracker = sender.gameObject.AddComponent<EntityTracker>();
        state.Tracker.TypeFilter = EntityTracker.TargetType.Enemy;
        state.Turret.GetComponent<SpriteRenderer>().sprite = TurretSprite;
        sender.State = state;

        if (sender.TryGetComponent<Health>(out var health))
        {
            health.Death += () => Instantiate(deathEffect, state.Turret.transform.position, new Quaternion());
        }
    }

    public override void OnUpdate(TowerController sender)
    {
    }

    public override void OnFixedUpdate(TowerController sender)
    {
        var state = (BasicTowerState)sender.State;
        if (state.Target == null)
        {
            var targetObject = state.Tracker.GetClosestEntity();
            if (targetObject == null)
                return;
            state.Target = targetObject.GetComponent<BasicEnemy>();
            if (state.Target == null)
                return;
        }
        var distance = Vector2.Distance(state.Turret.transform.position, state.Target.transform.position);
        var prefireVector = prefireCoefficient * distance
            * state.Target.Movement / projectile.GetComponent<Projectile>().Speed;
        var fireTarget = (Vector2)state.Target.transform.position + prefireVector;
        RotateTo(fireTarget, out var canFire);
        if (canFire)
        {
            if (state.FireCooldown >= 1 / fireRate)
            {
                LaunchProjectile();
                state.FireCooldown = 0;
            }
            else
                state.FireCooldown += Time.fixedDeltaTime;
        }


        void RotateTo(Vector2 fireTarget, out bool canFire)
        {
            var directionToTarget = fireTarget - (Vector2)state.Turret.transform.position;
            var angleToTarget = Vector2.SignedAngle(state.Turret.transform.up, directionToTarget);
            canFire = Math.Abs(angleToTarget) < fireAngleThreshold;
            float rotationAngle;
            if (Math.Abs(angleToTarget) < rotationSpeed)
                rotationAngle = angleToTarget;
            else
                rotationAngle = angleToTarget > 0 ? rotationSpeed : -rotationSpeed;
            state.Turret.transform.RotateAround(state.Base.transform.position, Vector3.forward, rotationAngle);
        }


        void LaunchProjectile()
        {
            Instantiate(projectile, state.BulletSpawnPoint.position, state.BulletSpawnPoint.rotation);
            Instantiate(shootEffect, state.BulletSpawnPoint.position, state.BulletSpawnPoint.rotation);
        }
    }
}
