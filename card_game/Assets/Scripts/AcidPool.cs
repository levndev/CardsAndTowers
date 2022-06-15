using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcidPool : MonoBehaviour
{
    public double Duration;
    public double Damage;
    public double DamageTickrate;

    public List<BasicEnemy> enemies;
    private double damageCooldown;
    private double timeAlive;

    void FixedUpdate()
    {
        if (timeAlive >= Duration)
            Destroy(transform.gameObject);
        else
            timeAlive += Time.fixedDeltaTime;

        // Deal damage to all enemies inside.
        if (damageCooldown >= 1 / DamageTickrate)
        {
            for (var i = enemies.Count - 1; i >= 0; i--)
                enemies[i].Health.TakeDamage(Damage);
        }
        else
            damageCooldown += Time.fixedDeltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
            enemies.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var enemy = other.transform.GetComponent<BasicEnemy>();
        if (enemy != null)
            enemies.Remove(enemy);
    }
}
