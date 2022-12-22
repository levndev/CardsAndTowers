using UnityEngine;

public class HitscanProjectile : MonoBehaviour
{
    public double Damage;
    public double Lifetime;

    private double timeElapsed;

    public void Activate(Transform tower, BasicEnemy enemy)
    {
        Strech(tower.position, enemy.transform.position, false);
        enemy.Health.TakeDamage(Damage);
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > Lifetime)
            Destroy(transform.gameObject);
    }

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

        var scale = new Vector3(Vector3.Distance(initialPosition, finalPosition), 1, 1);
        transform.localScale = scale;
    }
}
