using UnityEngine;

public class BasicHitscanProjectile : HitscanProjectile
{
    public double Damage;
    public double Lifetime;

    private double timeElapsed;

    public override void Activate(Vector3 start, BasicEnemy enemy)
    {
        Strech(start, enemy.transform.position, false);
        enemy.Health.TakeDamage(Damage);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
            Destroy(transform.gameObject);
    }
}

public abstract class HitscanProjectile : MonoBehaviour
{
    public abstract void Activate(Vector3 start, BasicEnemy enemy);

    public void Strech(Vector3 initialPosition, Vector3 finalPosition, bool mirrorZ)
    {
        var centerPos = (initialPosition + finalPosition) / 2f;
        transform.position = centerPos;

        var direction = (finalPosition - initialPosition).normalized;
        transform.right = direction;
        if (mirrorZ)
        {
            transform.right *= -1f;
        }

        var scale = new Vector3(Vector3.Distance(initialPosition, finalPosition) / 5.12f, 1, 1);
        transform.localScale = scale;
    }
}