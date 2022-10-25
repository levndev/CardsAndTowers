using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float RotationSpeed;
    public float Speed;
    public float ApproachDistance;
    public Health Health;
    public Vector2 Movement;
    public GameObject AttackEffect;

    private new Rigidbody2D rigidbody;
    private MapManager mapManager;
    private Vector3? goal = null;
    private bool aggroedOnTower = false;
    private Stack<Vector3> currentPath;
    private Queue<BasicTowerController> aggroedTowers;
    private List<Health> targets;
    public float AttackCooldown;
    public double AttackDamage;
    private float AttackCooldownTimeLeft;
    private bool CanAttack = true;
    private bool Stopped = false;
    private void Awake()
    {
        targets = new List<Health>();
        aggroedTowers = new Queue<BasicTowerController>();
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        //Health.Death += () => Destroy(transform.gameObject);
    }

    private void Update()
    {
        if (CanAttack)
        {
            if (targets.Count > 0)
            {
                if (targets[0] == null)
                {
                    targets.RemoveAt(0);
                }
                else
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
                            Stopped = false;
                    }
                    CanAttack = false;
                    AttackCooldownTimeLeft = AttackCooldown;
                }
            }
            else
            {
                if (Stopped)
                    Stopped = false;
            }
        }
        else
        {
            AttackCooldownTimeLeft -= Time.deltaTime;
            if (AttackCooldownTimeLeft < 0)
            {
                AttackCooldownTimeLeft = 0;
                CanAttack = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
        }
        if (Stopped)
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

    public void OnTowerRangeEnter(BasicTowerController tower)
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
        }
        aggroedTowers.Enqueue(tower);
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
            Stopped = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
    }
}
