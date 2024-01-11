using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Holds the data for the enemy gameobject, how many of the enemy, and the spawn positions when spawning
/// </summary>
[System.Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int count;
    public Transform[] spawnPoints;
}

/// <summary>
/// Hods the amount of enemy types in a wave, and the time between waves
/// </summary>
[System.Serializable]
public class Wave
{
    public EnemyWave[] enemies;
    public float timeBetweenWave; // Add this property
}

/// <summary>
/// Spawns the waves and manages the time between waves and the enemies incoming popup, and saving and loading of the waves
/// </summary>
public class WaveSpawner : MonoBehaviour, IDataPersistence
{
    public Wave[] waves;
    public float timeBetweenSpawns = 0.25f;
    [SerializeField] private float timeBeforeWavesStart = 5f;
    [SerializeField] private GameObject spawnEffect;
    [SerializeField] private Animator enemiesIncomingPopupAnimator;

    public int waveIndex = 0;
    private int totalTroopsSpawned = 0; // Counter variable
    private int expectedTotalTroops;

    private int savedWaveIndex;

    private static WaveSpawner _instance;
    public static WaveSpawner Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex != 1)
       StartCoroutine(WaitAndStartWaves(timeBeforeWavesStart));
        
    }

    public void LoadData(GameData data)
    {
        waveIndex = data.waveIndex;
    }

    public void SaveData(GameData data)
    {
        data.waveIndex = waveIndex;
    }

    public void StartWaves()
    {
        expectedTotalTroops = CalculateExpectedTotalTroops();
        StartCoroutine(SpawnWaves(waveIndex));

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

    public IEnumerator WaitAndStartWaves(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        StartWaves();
    }

    IEnumerator SpawnWaves(int waveStartIndex)
    {
        waveIndex = waveStartIndex;

        while (waveIndex < waves.Length)
        {
            Wave currentWave = waves[waveIndex];

            // Play "EnemiesIncomingPopup" 2 seconds before the wave
            //yield return new WaitForSeconds(currentWave.timeBetweenWave - 2f);
            EnemiesIncomingPopup();
            yield return new WaitForSeconds(3f);
            enemiesIncomingPopupAnimator.Play("Default");

            for (int i = 0; i < currentWave.enemies.Length; i++)
            {
                EnemyWave enemyWave = currentWave.enemies[i];
                yield return StartCoroutine(SpawnEnemies(enemyWave.enemyPrefab, enemyWave.count, enemyWave.spawnPoints));
            }

            if (waves.Length != 1)
            {
                yield return new WaitForSeconds(2f); // Wait for 2 seconds after the popup before starting the next wave
                yield return new WaitForSeconds(currentWave.timeBetweenWave - 2f);
            }

            waveIndex++;
        }

        // Check if all troops are spawned
        if (totalTroopsSpawned >= expectedTotalTroops)
        {
            // Assuming GameManager is a singleton or has a static instance property
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
//            Debug.Log("Troop spawned! Total: " + totalTroopsSpawned);
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    Transform GetRandomSpawnPoint(Transform[] spawnPoints)
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        return spawnPoints[randomIndex];
    }

    private void EnemiesIncomingPopup()
    {
        enemiesIncomingPopupAnimator.Play("EnemiesIncomingPopup");
      //  Debug.Log("Enemies incoming");
    }
}
