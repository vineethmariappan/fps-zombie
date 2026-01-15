using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaveManager : MonoBehaviour
{
    private int totalEnemiesThisWave = 0;

    public WaveUIManager waveUIManager;
    public Spawner spawner;
    public int currentWave = 0;
    private float waveTime = 120f;
    private float timer;

    public float timeBetweenWaves = 25f;

    private int baseEnemyCount = 8;
    private float enemyCountMultiplier = 1.5f;

    private int runnerUnlockWave = 2;
    private int shooterUnlockWave = 2;
    private int fastShooterUnlockWave = 3;

    private bool isWaveInProgress = false;

    void Start()
    {
        InitializeWaveSystem();
    }

    void Update()
    {
        ProcessWaveTimer();
    }

    public void StartNextWave()
    {
        InitiateNewWave();
    }

    public void StartFreeRoamPeriod()
    {
        StartCoroutine(FreeRoamCountdown());
    }

    public void OnEnemyKilled()
    {
        ProcessEnemyDeath();
    }

    public void CheckWaveCompletion()
    {
        EvaluateWaveStatus();
    }

    public void EndWave()
    {
        CompleteWave();
    }

    public void PlayerFailed()
    {
        HandleGameOver();
    }

    private void InitializeWaveSystem()
    {
        ActivateAllSpawners();
        StartNextWave();
    }

    private void ActivateAllSpawners()
    {
        for (int i = 0; i < spawner.spawners.Count; i++)
        {
            spawner.spawners[i].gameObject.SetActive(true);
        }
        spawner.RefreshActiveSpawners();
    }

    private void ProcessWaveTimer()
    {
        if (isWaveInProgress && timer > 0)
        {
            timer -= Time.deltaTime;
            waveUIManager.SetWaveTimeRemaining(timer);

            if (timer <= 0)
            {
                timer = 0;
                CheckWaveCompletion();
            }
        }
    }

    private void InitiateNewWave()
    {
        currentWave++;

        int enemiesThisWave = CalculateEnemyCount();
        ConfigureWaveEnemies(currentWave);
        BeginSpawning(enemiesThisWave);
        UpdateWaveUI(enemiesThisWave);
    }

    private int CalculateEnemyCount()
    {
        return Mathf.RoundToInt(baseEnemyCount * Mathf.Pow(enemyCountMultiplier, currentWave - 1));
    }

    private void ConfigureWaveEnemies(int wave)
    {
        SetAvailableEnemyTypes(wave);
    }

    private void BeginSpawning(int enemyCount)
    {
        spawner.StartSpawning(enemyCount);
        timer = waveTime;
        isWaveInProgress = true;
        totalEnemiesThisWave = enemyCount;
    }

    private void UpdateWaveUI(int enemyCount)
    {
        waveUIManager.SetEnemyCount(enemyCount, totalEnemiesThisWave);
        waveUIManager.SetWaveNumber(currentWave);
        waveUIManager.SetWaveTimeRemaining(timer);
        waveUIManager.DisplayWaveNotification($"Wave {currentWave} Started!");
    }

    private void SetAvailableEnemyTypes(int wave)
    {
        spawner.spawnableEnemies.Clear();

        if (wave == 1)
        {
            ConfigureWaveOneEnemies();
        }
        else if (wave == 2)
        {
            ConfigureWaveTwoEnemies();
        }
        else if (wave == 3)
        {
            ConfigureWaveThreeEnemies();
        }
        else if (wave <= 5)
        {
            ConfigureWaveFourToFiveEnemies();
        }
        else if (wave <= 8)
        {
            ConfigureWaveSixToEightEnemies();
        }
        else
        {
            ConfigureWaveNinePlusEnemies();
        }
    }

    private void ConfigureWaveOneEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
    }

    private void ConfigureWaveTwoEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
    }

    private void ConfigureWaveThreeEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
    }

    private void ConfigureWaveFourToFiveEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
    }

    private void ConfigureWaveSixToEightEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[0]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
    }

    private void ConfigureWaveNinePlusEnemies()
    {
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[1]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[2]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
        spawner.spawnableEnemies.Add(spawner.allEnemyPrefabs[3]);
    }

    private IEnumerator FreeRoamCountdown()
    {
        float freeRoamTimer = timeBetweenWaves;

        while (freeRoamTimer > 0)
        {
            freeRoamTimer -= Time.deltaTime;
            waveUIManager.DisplayFreeRoamTimeRemaining(freeRoamTimer);
            yield return null;
        }

        waveUIManager.DisplayWaveNotification("Wave Starting!", 1f);
        StartNextWave();
    }

    private void ProcessEnemyDeath()
    {
        spawner.activeEnemyCount--;
        waveUIManager.SetEnemyCount(spawner.activeEnemyCount, totalEnemiesThisWave);

        if (spawner.activeEnemyCount <= 0)
        {
            CheckWaveCompletion();
        }
    }

    private void EvaluateWaveStatus()
    {
        if (spawner.activeEnemyCount == 0 && !spawner.isSpawning)
        {
            waveUIManager.DisplayWaveNotification($"Wave {currentWave} Completed!");
            EndWave();
        }
        else if (timer <= 0)
        {
            waveUIManager.DisplayWaveNotification("Game Over! Wave Failed!");
            PlayerFailed();
        }
    }

    private void CompleteWave()
    {
        isWaveInProgress = false;

        float timeUsed = waveTime - timer;
        waveUIManager.DisplayWaveStats(currentWave, totalEnemiesThisWave, timeUsed);

        FindObjectOfType<PowerUps>().OnWaveEnd();
        StartFreeRoamPeriod();
    }

    private void HandleGameOver()
    {
        isWaveInProgress = false;
        PlayerPrefs.SetInt("WavesSurvived", currentWave - 1);
        SceneManager.LoadScene("GameEndScene");
        Time.timeScale = 1f;
    }
}