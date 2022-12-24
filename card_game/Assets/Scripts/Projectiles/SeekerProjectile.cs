using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekerProjectile : HitscanProjectile
{
    public float Speed;
    public double Damage;
    public double Lifetime;
    public float RotationSpeed;
    public float AggroRange;

    private double timeElapsed;
    private new Rigidbody2D rigidbody;
    private Transform target;

    private void Awake()
    {
        rigidbody = transform.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            MoveForward();
        }
        else
        {
            var direction = target.position - transform.position;
            var angleToTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            angleToTarget -= rigidbody.rotation;
            float rotationAngle;
            if (Math.Abs(angleToTarget) < RotationSpeed)
                rotationAngle = angleToTarget;
            else
                rotationAngle = angleToTarget > 0 ? RotationSpeed : -RotationSpeed;
            rigidbody.rotation += rotationAngle;
            MoveForward();
        }


        void MoveForward()
        {
            rigidbody.MovePosition(transform.position + Speed * Time.fixedDeltaTime * transform.up);
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
        {
            Destroy(transform.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BasicEnemy>(out var enemy))
        {
            enemy.Health.TakeDamage(Damage);
            target = FindNextEnemy();
        }
    }

    private Transform FindNextEnemy()
    {
        var colliders = Physics2D.OverlapBoxAll(
            transform.position,
            new Vector2(AggroRange, AggroRange), 0);
        float minDistance = float.MaxValue;
        BasicEnemy enemy = null;
        foreach (var collider in colliders)
        {
            var distance = Vector3.Distance(collider.transform.position, transform.position);
            if (collider.TryGetComponent<BasicEnemy>(out var newEnemy)
                && distance < minDistance
                && (target == null || target != newEnemy.transform))
            {
                enemy = newEnemy;
                minDistance = distance;
            }
        }
        return enemy != null ? enemy.transform : null;
    }

    public override void Activate(Vector3 start, BasicEnemy enemy)
    {
        target = enemy.transform;
    }
}
