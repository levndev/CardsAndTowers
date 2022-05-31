using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
{
    public float RotationSpeed;
    public float Speed;
    public float ApproachDistance;
    public double MaxHealth;
    public Health Health;
    public Vector2 Movement;

    private new Rigidbody2D rigidbody;
    private MapManager mapManager;
    private Vector3? goal = null;
    private bool aggroedOnTower = false;
    private Stack<Vector3> currentPath;
    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health = new Health(MaxHealth, transform.Find("Healthbar").gameObject);
        Health.Death += () => Destroy(transform.gameObject);
    }

    private void FixedUpdate()
    {
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
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
        if (aggroedOnTower)
            return;
        if (mapManager == null)
        {
            mapManager = GameManager.Instance.MapManager;
        }
        aggroedOnTower = true;
        currentPath = mapManager.GetPath(transform.position, tower.transform.position);
        goal = null;
    }
}
