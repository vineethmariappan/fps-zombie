using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private PowerUps powerUpManager;

    private void Start()
    {
        InitializeManager();
    }

    public void InitializePowerUp(PowerUps manager)
    {
        powerUpManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ProcessPlayerCollection(other.gameObject);
        }
    }

    private void InitializeManager()
    {
        if (powerUpManager == null)
        {
            powerUpManager = FindObjectOfType<PowerUps>();
        }
    }

    private void ProcessPlayerCollection(GameObject player)
    {
        if (powerUpManager == null)
        {
            return;
        }

        powerUpManager.ApplyPowerUpEffect(gameObject, player);
        DestroyPowerUp();
    }

    private void DestroyPowerUp()
    {
        Destroy(transform.parent.gameObject);
    }
}