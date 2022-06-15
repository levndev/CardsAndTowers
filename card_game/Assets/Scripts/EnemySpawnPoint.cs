using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    public double SpawnRate;
    public bool Enabled;
    public GameObject[] Enemies;

    private double spawnCooldown;
    private int nextEnemyIndex;

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
        Instantiate(Enemies[nextEnemyIndex], transform.position, transform.rotation);
        nextEnemyIndex = (nextEnemyIndex + 1) % Enemies.Length;
    }
}
