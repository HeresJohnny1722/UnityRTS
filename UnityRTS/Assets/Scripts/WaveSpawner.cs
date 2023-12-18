using System.Collections;
using UnityEngine;

[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count;
    public Transform[] spawnPoints;
}

[System.Serializable]
public class Wave
{
    public EnemyWave[] enemies;
}

public class WaveSpawner : MonoBehaviour
{
    public Wave[] waves;
    public float timeBetweenWaves = 5f;
    public float timeBetweenSpawns = .25f;

    private int waveIndex = 0;
    private int totalTroopsSpawned = 0; // New counter variable
    private int expectedTotalTroops; // New variable to store the expected total troops

    public void StartWaves()
    {
        // Calculate the expected total troops
        foreach (Wave wave in waves)
        {
            foreach (EnemyWave enemyWave in wave.enemies)
            {
                expectedTotalTroops += enemyWave.count;
            }
        }

        StartCoroutine(SpawnWave());
    }

    IEnumerator SpawnWave()
    {
        while (waveIndex < waves.Length)
        {
            Wave currentWave = waves[waveIndex];

            for (int i = 0; i < currentWave.enemies.Length; i++)
            {
                EnemyWave enemyWave = currentWave.enemies[i];
                StartCoroutine(SpawnEnemies(enemyWave.enemyPrefab, enemyWave.count, enemyWave.spawnPoints));
                yield return new WaitForSeconds(1f);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
            waveIndex++;
        }

        // Check if all troops are spawned
        if (totalTroopsSpawned >= expectedTotalTroops)
        {
            Debug.Log("Game is over, you win!");
            GameManager.instance.areWavesDone = true;
        }
    }

    IEnumerator SpawnEnemies(GameObject enemyPrefab, int count, Transform[] spawnPoints)
    {
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = GetRandomSpawnPoint(spawnPoints);
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            totalTroopsSpawned++; // Increment the counter when a troop is spawned
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
}
