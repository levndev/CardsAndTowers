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
    public float CurrentSpeed;

    private new Rigidbody2D rigidbody;
    private MapManager mapManager;
    private Vector3? goal = null;
    private Vector3 oldPosition;

    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Health = new Health(MaxHealth);
        Health.Death += () => Destroy(transform.gameObject);
    }

    private void FixedUpdate()
    {
        CurrentSpeed = (transform.position - oldPosition).magnitude;
        oldPosition = transform.position;

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
                goal = null;
                return;
            }
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
            goal = mapManager.GetPath(transform.position);
        }
    }
}
