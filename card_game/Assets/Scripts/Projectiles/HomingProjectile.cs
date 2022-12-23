using UnityEngine;

public class HomingProjectile : HitscanProjectile
{
    public float Speed;
    public double Damage;
    public double Lifetime;

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
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rigidbody.SetRotation(angle - 90);
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
            Explode(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<BasicEnemy>(out var enemy))
        {
            Explode(enemy);
        }
    }

    protected virtual void Explode(BasicEnemy enemy)
    {
        if (enemy != null)
        {
            enemy.Health.TakeDamage(Damage);
        }
        Destroy(transform.gameObject);
    }

    public override void Activate(Vector3 start, BasicEnemy enemy)
    {
        target = enemy.transform;
    }
}
