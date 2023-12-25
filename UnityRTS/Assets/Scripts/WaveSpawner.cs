using System.Collections;
using UnityEngine;
using TMPro;

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
    public float timeBetweenSpawns = 0.25f;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private TextMeshProUGUI waveCounterText;

    private int waveIndex = 0;
    private int totalTroopsSpawned = 0; // Counter variable
    private int expectedTotalTroops;



    public void StartWaves()
    {
        expectedTotalTroops = CalculateExpectedTotalTroops();
        StartCoroutine(SpawnWaves());
    }

    private void Update()
    {
        int waveCount = waveIndex + 1;
        waveCounterText.text = "Wave: " + waveCount;
    }

    int CalculateExpectedTotalTroops()
    {
        int total = 0;
        foreach (Wave wave in waves)
        {
            foreach (EnemyWave enemyWave in wave.enemies)
            {
                total += enemyWave.count;
            }
        }
        return total;
    }

    IEnumerator SpawnWaves()
    {
        while (waveIndex < waves.Length)
        {
            Wave currentWave = waves[waveIndex];

            for (int i = 0; i < currentWave.enemies.Length; i++)
            {
                EnemyWave enemyWave = currentWave.enemies[i];
                yield return StartCoroutine(SpawnEnemies(enemyWave.enemyPrefab, enemyWave.count, enemyWave.spawnPoints));
            }

            if (waves.Length != 1)
            {
                yield return new WaitForSeconds(timeBetweenWaves);
            }

            waveIndex++;
        }

        // Check if all troops are spawned
        if (totalTroopsSpawned >= expectedTotalTroops)
        {
            Debug.Log("Waves are done spawning");
            GameManager.instance.areWavesDone = true;
        }
    }

    IEnumerator SpawnEnemies(GameObject enemyPrefab, int count, Transform[] spawnPoints)
    {
        for (int i = 0; i < count; i++)
        {
            Transform spawnPoint = GetRandomSpawnPoint(spawnPoints);
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            GameObject spawnObject = Instantiate(spawnEffect, spawnPoint.position, Quaternion.identity);
            Destroy(spawnObject, 5f);
            totalTroopsSpawned++; // Increment the counter when a troop is spawned
            Debug.Log("Troop spawned! Total: " + totalTroopsSpawned);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }
}