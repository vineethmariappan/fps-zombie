using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private WaveManager waveManager;
    
    void Start()
    {
        InitializeHealth();
    }

    public void TakeDamage(float amount)
    {
        ApplyDamage(amount);
    }

    private void InitializeHealth()
    {
        currentHealth = maxHealth;
        waveManager = FindObjectOfType<WaveManager>();
    }

    private void ApplyDamage(float damageAmount)
    {
        currentHealth -= damageAmount;
        
        if (currentHealth <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        if (waveManager != null)
        {
            waveManager.OnEnemyKilled();
        }
        
        Destroy(gameObject);
    }
}