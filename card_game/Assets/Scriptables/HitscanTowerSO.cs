using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CardsAndTowers/Hitscan Tower")]
public class HitscanTowerSO : TowerSO
{
    private class HitscanTowerState : TowerState
    {
        public GameObject Turret;
        public GameObject Base;
        public BasicEnemy Target;
        public double FireCooldown;
        public Transform BulletSpawnPoint;
        public EntityTracker Tracker;
    }
    [SerializeField] private double fireRate;
    [SerializeField] private HitscanProjectile projectile;
    [SerializeField] private float rangeRadius;
    [SerializeField] private Sprite TurretSprite;
    [SerializeField] private GameObject shootEffect;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private float bulletSpawnPointHeight;

    public override void Init(TowerController sender)
    {
        base.Init(sender);
        var state = new HitscanTowerState()
        {
            Turret = sender.transform.Find("Turret").gameObject,
            Base = sender.transform.Find("Base").gameObject,
        };
        state.BulletSpawnPoint = state.Turret.transform.Find("BulletSpawnPoint");
        state.BulletSpawnPoint.localPosition = new Vector3(0, bulletSpawnPointHeight, 0);
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
        var state = (HitscanTowerState)sender.State;
        if (state.Target == null)
        {
            var targetObject = state.Tracker.GetClosestEntity();
            if (targetObject == null)
                return;
            state.Target = targetObject.GetComponent<BasicEnemy>();
            if (state.Target == null)
                return;
        }

        RotateTo(state.Target.transform.position);
        if (state.FireCooldown >= 1 / fireRate)
        {
            Shoot();
            state.FireCooldown = 0;
        }
        else
            state.FireCooldown += Time.fixedDeltaTime;


        void RotateTo(Vector2 fireTarget)
        {
            var directionToTarget = fireTarget - (Vector2)state.Turret.transform.position;
            var angleToTarget = Vector2.SignedAngle(state.Turret.transform.up, directionToTarget);
            state.Turret.transform.RotateAround(state.Base.transform.position, Vector3.forward, angleToTarget);
        }

            
        void Shoot()
        {
            var hit = Instantiate(projectile, state.BulletSpawnPoint.position, state.BulletSpawnPoint.rotation);
            hit.Activate(state.Turret.transform, state.Target);
            Instantiate(shootEffect, state.BulletSpawnPoint.position, state.BulletSpawnPoint.rotation);
        }
    }
}
