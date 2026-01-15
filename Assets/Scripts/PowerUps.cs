using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PowerUps : MonoBehaviour
{
    public GameObject[] collectableSpawners;
    public GameObject[] powerUpPrefabs;
    public float spawnDuration = 100f;
    private List<GameObject> activePowerUps = new List<GameObject>();
    private bool isSpawningActive = false;
    public int numberOfPowerUpsToSpawn = 4;

    private WaveManager waveManager;
    public WaveUIManager waveUIManager;

    public AudioClip powerUpSound;
    public AudioClip spawnSound;
    private AudioSource audioSource;

    private void Start()
    {
        InitializeComponents();
    }

    public void StartPowerUpSpawning()
    {
        if (!isSpawningActive)
        {
            StartCoroutine(SpawnPowerUpsForDuration());
        }
    }

    public void OnWaveEnd()
    {
        StartPowerUpSpawning();
    }

    public void ApplyPowerUpEffect(GameObject powerUp, GameObject player)
    {
        string powerUpTag = powerUp.tag;
        PlayPowerUpSound();

        ExecutePowerUpEffect(powerUpTag, player);
    }

    private void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
        waveManager = FindObjectOfType<WaveManager>();
    }

    private IEnumerator SpawnPowerUpsForDuration()
    {
        isSpawningActive = true;
        activePowerUps.Clear();

        SpawnPowerUpsAtRandomLocations();
        PlayPowerUpSpawnSound();

        yield return new WaitForSeconds(spawnDuration);

        DestroyActivePowerUps();

        activePowerUps.Clear();
        isSpawningActive = false;
    }

    private void SpawnPowerUpsAtRandomLocations()
    {
        List<GameObject> shuffledSpawners = ShuffleSpawners();
        int numToSpawn = Mathf.Min(numberOfPowerUpsToSpawn, shuffledSpawners.Count);

        for (int i = 0; i < numToSpawn; i++)
        {
            InstantiatePowerUp(shuffledSpawners[i]);
        }
    }

    private List<GameObject> ShuffleSpawners()
    {
        List<GameObject> shuffledSpawners = new List<GameObject>(collectableSpawners);
        System.Random rng = new System.Random();
        return shuffledSpawners.OrderBy(x => rng.Next()).ToList();
    }

    private void InstantiatePowerUp(GameObject spawner)
    {
        GameObject randomPowerUp = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        GameObject spawnedPowerUp = Instantiate(randomPowerUp, spawner.transform.position, spawner.transform.rotation);
        activePowerUps.Add(spawnedPowerUp);
    }

    private void DestroyActivePowerUps()
    {
        foreach (GameObject powerUp in activePowerUps)
        {
            if (powerUp != null)
            {
                Destroy(powerUp);
            }
        }
    }

    private void ExecutePowerUpEffect(string powerUpTag, GameObject player)
    {
        switch (powerUpTag)
        {
            case "HealthSphear":
                ApplyHealthRefill(player);
                break;

            case "KillSphear":
                KillAllEnemies();
                break;

            case "RandomSphear":
                RandomEffect(player);
                break;

            case "AmmoSphear":
                ApplyBulletRefill(player);
                break;

            case "RocketSphear":
                ApplyShootingSpeedBoost(player);
                break;
        }
    }

    private void RandomEffect(GameObject player)
    {
        int randomEffect = Random.Range(0, 3);

        switch (randomEffect)
        {
            case 0:
                ApplyHealthRefill(player);
                break;

            case 1:
                KillAllEnemies();
                break;

            case 2:
                KillRandomEnemies();
                break;
        }
    }

    private void ApplyHealthRefill(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.RestoreHealth();
            waveUIManager.DisplayWaveNotification("Player's health restored!");
        }
    }

    private void ApplyBulletRefill(GameObject player)
    {
        Gun gun = FindPlayerGun(player);

        if (gun != null)
        {
            gun.bulletsIHave += gun.amountOfBulletsPerLoad * 3;
            waveUIManager.DisplayWaveNotification("Ammo refilled!");
        }
    }

    private void ApplyShootingSpeedBoost(GameObject player)
    {
        Gun gun = FindPlayerGun(player);

        if (gun != null)
        {
            StartCoroutine(ShootingSpeedBoostCoroutine(gun));
            waveUIManager.DisplayWaveNotification("Fire rate boosted!");
        }
    }

    private Gun FindPlayerGun(GameObject player)
    {
        Gun gun = player.GetComponentInChildren<Gun>();

        if (gun == null)
        {
            GunInventory inventory = player.GetComponent<GunInventory>();
            if (inventory != null && inventory.currentGun != null)
            {
                gun = inventory.currentGun.GetComponent<Gun>();
            }
        }

        return gun;
    }

    private IEnumerator ShootingSpeedBoostCoroutine(Gun gun)
    {
        float originalFireRate = gun.roundsPerSecond;
        gun.roundsPerSecond *= 2f;

        yield return new WaitForSeconds(20f);

        gun.roundsPerSecond = originalFireRate;
    }

    private void KillAllEnemies()
    {
        ZombieWalkAI[] enemies = FindObjectsOfType<ZombieWalkAI>();
        
        foreach (ZombieWalkAI enemy in enemies)
        {
            if (waveManager != null)
            {
                waveManager.OnEnemyKilled();
            }
            Destroy(enemy.gameObject);
        }
        
        waveUIManager.DisplayWaveNotification("All enemies killed!");
    }

    private void KillRandomEnemies()
    {
        ZombieWalkAI[] enemies = FindObjectsOfType<ZombieWalkAI>();
        
        if (enemies.Length > 0)
        {
            int enemiesToKill = Random.Range(0, enemies.Length);
            
            for (int i = 0; i < enemiesToKill; i++)
            {
                if (enemies.Length > 0)
                {
                    int randomIndex = Random.Range(0, enemies.Length);
                    Destroy(enemies[randomIndex].gameObject);
                    enemies = FindObjectsOfType<ZombieWalkAI>();
                }
            }
            
            waveUIManager.DisplayWaveNotification("Killed " + enemiesToKill + " enemies!");
        }
    }

    private void PlayPowerUpSound()
    {
        if (audioSource != null && powerUpSound != null)
        {
            audioSource.PlayOneShot(powerUpSound);
        }
    }

    private void PlayPowerUpSpawnSound()
    {
        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }
}