using UnityEngine;

public class ChainProjectile : HitscanProjectile
{
    public double Damage;
    public double Lifetime;
    public int MaxHitCount;
    public float MaxHitRadius;

    private double timeElapsed;
    private Vector3 currentEnemyPosition;
    private BasicEnemy currentEnemy;

    public int HitCount { get; set; } = -1;

    public override void Activate(Vector3 start, BasicEnemy enemy)
    {
        if (HitCount == -1)
        {
            HitCount = MaxHitCount;
        }
        Strech(start, enemy.transform.position, false);
        enemy.Health.TakeDamage(Damage);
        currentEnemy = enemy;
        currentEnemyPosition = enemy.transform.position;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
        {
            SpawnNextHit();
            Destroy(transform.gameObject);
        }
    }

    private void SpawnNextHit()
    {
        if (HitCount == 0)
        {
            return;
        }
        if (TryFindNextEnemy(out var nextEnemy))
        {
            var newProjectile = Instantiate(this);
            newProjectile.HitCount = HitCount - 1;
            newProjectile.Activate(currentEnemyPosition, nextEnemy);
        }


        bool TryFindNextEnemy(out BasicEnemy enemy)
        {
            enemy = default;
            var colliders = Physics2D.OverlapBoxAll(currentEnemyPosition, new Vector2(MaxHitRadius, MaxHitRadius), 0);
            float minDistance = float.MaxValue;
            foreach (var collider in colliders)
            {
                var distance = Vector3.Distance(collider.transform.position, currentEnemyPosition);
                if (collider.TryGetComponent<BasicEnemy>(out var newEnemy)
                    && distance < MaxHitRadius
                    && distance < minDistance
                    && (currentEnemy == null || currentEnemy != newEnemy))
                {
                    enemy = newEnemy;
                    minDistance = distance;
                }
            }
            return minDistance != float.MaxValue;
        }
    }
}
