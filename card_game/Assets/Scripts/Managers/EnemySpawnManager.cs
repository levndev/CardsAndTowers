using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnPoint> spawnPoints;
    [SerializeField] private List<Spawn> possibleEnemies;
    [SerializeField] private float spawnCooldown;
    [SerializeField] private float levelDifficulty;
    [SerializeField] private AnimationCurve difficultyScaling;

    private double timeSinceLastSpawn;

    private void Update()
    {
        if (timeSinceLastSpawn > spawnCooldown)
        {
            timeSinceLastSpawn = 0;
            SpawnEnemies();
        }
        timeSinceLastSpawn += Time.deltaTime;
    }

    private void SpawnEnemies()
    {
        var mapProgress = (GameManager.Instance.TimeToWin - GameManager.Instance.TimeToWinLeft)
            / GameManager.Instance.TimeToWin;
        var currentDifficulty = levelDifficulty * difficultyScaling.Evaluate(mapProgress) * spawnCooldown;
        var difficultyPointsLeft = currentDifficulty;
        while(difficultyPointsLeft > 0)
        {
            var randomSpawn = possibleEnemies.Where(spawn => spawn.Enemy.Difficulty < difficultyPointsLeft)
                .ChooseRandom(spawn => spawn.Modifier / spawn.Enemy.Difficulty);
            if (randomSpawn == null)
                return;
            var enemy = randomSpawn.Enemy;
            var spawnPoint = spawnPoints.ChooseRandom(spawnPoint => spawnPoint.Weight);
            Instantiate(enemy, spawnPoint.transform.position, new Quaternion());
            difficultyPointsLeft -= enemy.Difficulty;
        }
    }

    [Serializable]
    private class Spawn
    {
        [Tooltip("0 - never spawns, 1 - spawns normally."), Range(0f, 1f)]
        public float Modifier;
        public BasicEnemy Enemy;
    }
}

