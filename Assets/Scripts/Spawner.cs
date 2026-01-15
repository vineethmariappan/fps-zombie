using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public List<Transform> spawners;
    public List<GameObject> allEnemyPrefabs;
    public List<GameObject> spawnableEnemies = new List<GameObject>();
    public Transform player;
    public int activeEnemyCount = 0;

    private List<Transform> activeSpawners = new List<Transform>();
    public float spawnInterval = 2f;
    public bool isSpawning = false;

    void Start()
    {
        InitializeSpawners();
    }

    public void RefreshActiveSpawners()
    {
        UpdateActiveSpawnersList();
    }

    public void StartSpawning(int maxEnemiesToSpawn)
    {
        if (!isSpawning)
        {
            StartCoroutine(SpawnEnemies(maxEnemiesToSpawn));
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    public void TeleportEnemiesToSpawners()
    {
        RelocateAllEnemies();
    }

    private void InitializeSpawners()
    {
        RefreshActiveSpawners();
    }

    private void UpdateActiveSpawnersList()
    {
        activeSpawners.Clear();
        
        foreach (var spawner in spawners)
        {
            if (spawner.gameObject.activeSelf)
            {
                activeSpawners.Add(spawner);
            }
        }
    }

    private IEnumerator SpawnEnemies(int maxEnemiesToSpawn)
    {
        isSpawning = true;
        int enemiesSpawned = 0;

        while (isSpawning && enemiesSpawned < maxEnemiesToSpawn)
        {
            if (!CanSpawn())
            {
                break;
            }

            SpawnSingleEnemy();
            enemiesSpawned++;
            activeEnemyCount++;

            yield return new WaitForSeconds(spawnInterval);
        }

        isSpawning = false;
    }

    private bool CanSpawn()
    {
        if (activeSpawners.Count == 0 || spawnableEnemies.Count == 0)
        {
            return false;
        }
        return true;
    }

    private void SpawnSingleEnemy()
    {
        Transform spawnLocation = GetRandomSpawner();
        GameObject enemyPrefab = GetRandomEnemyPrefab();

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnLocation.position, Quaternion.identity);
        AssignPlayerToEnemy(spawnedEnemy);
    }

    private Transform GetRandomSpawner()
    {
        return activeSpawners[Random.Range(0, activeSpawners.Count)];
    }

    private GameObject GetRandomEnemyPrefab()
    {
        return spawnableEnemies[Random.Range(0, spawnableEnemies.Count)];
    }

    private void AssignPlayerToEnemy(GameObject enemy)
    {
        ZombieWalkAI zombieWalkAI = enemy.GetComponent<ZombieWalkAI>();
        if (zombieWalkAI != null)
        {
            zombieWalkAI.SetPlayer(player);
        }

        ZombieRunAI zombieRunAI = enemy.GetComponent<ZombieRunAI>();
        if (zombieRunAI != null)
        {
            zombieRunAI.SetPlayer(player);
        }

        ZombieShooter zombieShooter = enemy.GetComponent<ZombieShooter>();
        if (zombieShooter != null)
        {
            zombieShooter.SetPlayer(player);
        }
    }

    private void RelocateAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        if (activeSpawners.Count == 0)
        {
            return;
        }

        foreach (var enemy in enemies)
        {
            Transform randomSpawner = GetRandomSpawner();
            enemy.transform.position = randomSpawner.position;
        }
    }
}