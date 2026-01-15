using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public GameObject[] lives;
    private int currentLives;

    public Slider healthSlider;
    public GameObject deathMessagePanel;
    public TMP_Text deathMessage;
    public TMP_Text respawnText;
    public GameObject respawnPlatform;
    public Transform respawnPoint;
    public WaveManager waveManager;
    public WaveUIManager waveUIManager;
    public Spawner spawner;
    private Gun gun;
    private GunInventory gunInventory;

    private bool isRespawning = false;

    void Start()
    {
        InitializePlayer();
    }

    void Update()
    {
        CheckGameOverCondition();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isRespawning) return;

        ApplyDamage(damageAmount);
        PlayHurtFeedback();
        RefreshHealthUI();

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth;
            StartCoroutine(HandlePlayerDeath());
        }
    }

    public void RestoreHealth()
    {
        currentHealth = maxHealth;
        RefreshHealthUI();
    }

    public void GameOver()
    {
        waveUIManager.DisplayWaveNotification("Game Over! Wave Failed!");
        waveManager.PlayerFailed();
    }

    private void InitializePlayer()
    {
        gunInventory = FindObjectOfType<GunInventory>();
        currentHealth = maxHealth;
        currentLives = lives.Length;
        RefreshHealthUI();

        if (deathMessagePanel != null) deathMessagePanel.SetActive(false);
        if (respawnPlatform != null) respawnPlatform.SetActive(false);
    }

    private void CheckGameOverCondition()
    {
        if (currentLives <= 0 && currentHealth <= 0)
        {
            waveManager.PlayerFailed();
        }
    }

    private void ApplyDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void PlayHurtFeedback()
    {
        PlayerMovement playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.PlayHurtSound();
        }
    }

    private void RefreshHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth / maxHealth;
        }
    }

    private IEnumerator HandlePlayerDeath()
    {
        isRespawning = true;
        DecrementLife();

        if (currentLives > 0)
        {
            yield return ProcessRespawnSequence();
        }
        else
        {
            GameOver();
        }

        isRespawning = false;
    }

    private IEnumerator ProcessRespawnSequence()
    {
        ShowDeathScreen();
        PauseGameplay();

        yield return CountdownRespawn();

        HideDeathScreen();
        ResumeGameplay();

        ActivateRespawnPlatform();
        ResetEnemies();
        ExecuteRespawn();

        yield return new WaitForSecondsRealtime(3f);
        DeactivateRespawnPlatform();
    }

    private void ShowDeathScreen()
    {
        if (deathMessagePanel != null)
        {
            MouseLook.isUIActive = true;
            MouseLook.SetCursorState(true);

            EnsureGunReference();
            HideGameplayUI();

            deathMessagePanel.SetActive(true);
            deathMessage.text = "You Died!";
        }
    }

    private void PauseGameplay()
    {
        Time.timeScale = 0f;
    }

    private IEnumerator CountdownRespawn()
    {
        for (int i = 5; i > 0; i--)
        {
            if (respawnText != null)
            {
                respawnText.text = $"Respawning in {i} seconds...";
            }
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void HideDeathScreen()
    {
        if (deathMessagePanel != null)
        {
            deathMessagePanel.SetActive(false);
        }
    }

    private void ResumeGameplay()
    {
        MouseLook.isUIActive = false;
        MouseLook.SetCursorState(false);

        EnsureGunReference();
        ShowGameplayUI();
    }

    private void ActivateRespawnPlatform()
    {
        if (respawnPlatform != null)
        {
            respawnPlatform.SetActive(true);
        }
    }

    private void ResetEnemies()
    {
        if (spawner != null)
        {
            spawner.RefreshActiveSpawners();
            spawner.TeleportEnemiesToSpawners();
        }
    }

    private void ExecuteRespawn()
    {
        RespawnPlayer();
        Time.timeScale = 1f;
    }

    private void DeactivateRespawnPlatform()
    {
        if (respawnPlatform != null)
        {
            respawnPlatform.SetActive(false);
        }
    }

    private void DecrementLife()
    {
        if (currentLives > 0)
        {
            currentLives--;
            Destroy(lives[currentLives]);
        }
    }

    private void RespawnPlayer()
    {
        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        currentHealth = maxHealth;
        RefreshHealthUI();
    }

    private void EnsureGunReference()
    {
        if (gun == null)
        {
            gun = FindObjectOfType<Gun>();
        }
    }

    private void HideGameplayUI()
    {
        if (gunInventory != null)
        {
            gunInventory.isInventoryVisible = false;
        }

        if (gun != null)
        {
            gun.isCrosshairVisible = false;
        }
    }

    private void ShowGameplayUI()
    {
        if (gunInventory != null)
        {
            gunInventory.isInventoryVisible = true;
        }

        if (gun != null)
        {
            gun.isCrosshairVisible = true;
        }
    }
}