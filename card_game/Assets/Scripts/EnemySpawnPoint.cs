using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public double SpawnRate;
    public bool Enabled;
    public GameObject Enemy;

    private double spawnCooldown;

    private void FixedUpdate()
    {
        if (!Enabled)
            return;
        if (spawnCooldown >= SpawnRate)
        {
            Spawn();
            spawnCooldown = 0;
        }
        else
            spawnCooldown += Time.fixedDeltaTime;
    }

    private void Spawn()
    {
        Instantiate(Enemy, transform.position, transform.rotation);
    }
}
