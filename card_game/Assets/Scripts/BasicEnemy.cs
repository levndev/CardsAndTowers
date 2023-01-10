using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class BasicEnemy : MonoBehaviour
{
    public float RotationSpeed;
    public float Speed;
    public float ApproachDistance;
    public Health Health;
    public Vector2 Movement;
    public GameObject AttackEffect;
    public GameObject DeathEffect;
    public GameObject HitEffect;
    [Tooltip("Used for calculating enemy spawns.")]
    public float Difficulty;

    private new Rigidbody2D rigidbody;
    private MapManager mapManager;
    private Vector3? goal = null;
    private bool aggroedOnTower = false;
    private Stack<Vector3> currentPath;
    private SimplePriorityQueue<TowerController, int> aggroedTowers;
    private List<Health> targets;
    public float AttackCooldown;
    public double AttackDamage;
    private float AttackCooldownTimeLeft;
    private bool CanAttack = true;
    private bool Fighting = false;

    private void Awake()
    {
        targets = new List<Health>();
        aggroedTowers = new();
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health.Death += () => Instantiate(DeathEffect, transform.position, new Quaternion());
        Health.Damaged += () => Instantiate(HitEffect, transform.position, new Quaternion());
    }

    private void Update()
    {
        if (!CanAttack)
        {
            AttackCooldownTimeLeft -= Time.deltaTime;
            if (AttackCooldownTimeLeft < 0)
            {
                AttackCooldownTimeLeft = 0;
                CanAttack = true;
            }
        }
        targets.RemoveAll(i => i == null);
        if (targets.Count == 0)
        {
            Fighting = false;
            return;
        }
        if (CanAttack)
        {
            if (AttackEffect != null)
            {
                var attackPoint = (targets[0].gameObject.transform.position + transform.position) / 2;
                Instantiate(AttackEffect, attackPoint, new Quaternion());
            }
            if (targets[0].TakeDamage(AttackDamage) <= 0)
            {
                targets.RemoveAt(0);
                if (targets.Count == 0)
                    Fighting = false;
            }
            CanAttack = false;
            AttackCooldownTimeLeft = AttackCooldown;
        }
    }

    private void FixedUpdate()
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
        }
        if (Fighting)
        {
            Movement = Vector2.zero;
            return;
        }
        if (aggroedTowers.Count > 0 && !aggroedOnTower)
        {
            var tower = aggroedTowers.Dequeue();
            if (tower != null)
            {
                tower.gameObject.GetComponent<Health>().Death += OnTargetTowerDeath;
                aggroedOnTower = true;
                currentPath = mapManager.GetPath(transform.position, tower.transform.position);
                goal = null;
            }
            else
            {
                return;
            }
        }
        if (goal.HasValue)
        {
            var heading = goal.Value - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance;
            if (distance < ApproachDistance)
            {
                goal = GetNextNode();
                if (((Vector3)goal - transform.position).magnitude < 0.01f)
                    Movement = Vector2.zero;
                return;
            }
            Movement = Speed * Time.fixedDeltaTime * direction;
            var angleToTarget = Vector3.SignedAngle(transform.up, direction, Vector3.forward);
            float rotationAngle;
            if (Mathf.Abs(angleToTarget) < RotationSpeed)
                rotationAngle = angleToTarget;
            else
                rotationAngle = angleToTarget > 0 ? RotationSpeed : -RotationSpeed;
            rigidbody.MoveRotation(rigidbody.rotation + rotationAngle * RotationSpeed * Time.fixedDeltaTime);
            rigidbody.MovePosition(transform.position + direction * Time.fixedDeltaTime * Speed);
        }
        else
        {
            goal = GetNextNode();
        }
    }

    private Vector3 GetNextNode()
    {
        Vector3 next;
        if (aggroedOnTower)
        {
            if (currentPath != null && currentPath.Count > 0)
                next = currentPath.Pop();
            else
                return transform.position;
        }
        else
        {
            next = mapManager.GetPathToBase(transform.position);
        }
        return next;
    }

    public void OnTowerRangeEnter(TowerController tower, int priority)
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
        }
        aggroedTowers.Enqueue(tower, priority);
    }

    public void OnTargetTowerDeath()
    {
        aggroedOnTower = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Destructible" && !other.isTrigger)
        {
            var health = other.gameObject.GetComponent<Health>();
            targets.Add(health);
            Fighting = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
    }
}
