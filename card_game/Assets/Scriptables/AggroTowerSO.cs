using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CardsAndTowers/Aggro Tower")]
public class AggroTowerSO : TowerSO
{
    private class AggroTowerState : TowerState
    {
        public GameObject Turret;
        public EntityTracker Tracker;
    }
    [SerializeField] private Sprite TurretSprite;
    [SerializeField] protected int AggroPriority;
    [SerializeField] protected int AggroRange;
    [SerializeField] private GameObject deathEffect;
    [SerializeField] private int MaxHealth;
    [SerializeField] private float RotationSpeed;
    public override void Init(TowerController sender)
    {
        base.Init(sender);
        var state = new AggroTowerState()
        {
            Turret = sender.transform.Find("Turret").gameObject,
        };
        var collider = sender.gameObject.AddComponent<CircleCollider2D>();
        collider.radius = AggroRange;
        collider.isTrigger = true;
        state.Tracker = sender.gameObject.AddComponent<EntityTracker>();
        state.Tracker.OnEntityEnterRange.AddListener(OnEnemyEnterRange);
        state.Tracker.TypeFilter = EntityTracker.TargetType.Enemy;
        state.Turret.GetComponent<SpriteRenderer>().sprite = TurretSprite;
        sender.State = state;
        if (sender.TryGetComponent<Health>(out var health))
        {
            health.Max = MaxHealth;
            health.Death += () => Instantiate(deathEffect, state.Turret.transform.position, new Quaternion());
        }
    }

    public void OnEnemyEnterRange(EntityTracker sender, GameObject entity)
    {
        var tower = sender.gameObject.GetComponent<TowerController>();
        var enemy = entity.GetComponent<BasicEnemy>();
        if (enemy != null)
        {
            enemy.OnTowerRangeEnter(tower, AggroPriority);
        }
    }

    public override void OnUpdate(TowerController sender)
    {
        var state = (AggroTowerState)sender.State;
        state.Turret.transform.Rotate(Vector3.forward, RotationSpeed * Time.deltaTime);
    }

    public override void OnFixedUpdate(TowerController sender)
    {
    }
}
