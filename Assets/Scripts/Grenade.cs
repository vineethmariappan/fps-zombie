using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float explosionDelay = 3f;
    public float explosionRadius = 10f;
    public float explosionForce = 700f;
    public float damage = 100f;
    
    [Header("Effects")]
    public GameObject explosionEffect;
    
    private bool hasDetonated = false;
    private float timer;

    void Start()
    {
        InitializeTimer();
    }

    void Update()
    {
        UpdateTimer();
    }

    private void InitializeTimer()
    {
        timer = explosionDelay;
    }

    private void UpdateTimer()
    {
        timer -= Time.deltaTime;
        
        if (timer <= 0f && !hasDetonated)
        {
            TriggerExplosion();
        }
    }

    private void TriggerExplosion()
    {
        hasDetonated = true;
        SpawnExplosionEffect();
        ProcessExplosionDamage();
        Destroy(gameObject);
    }

    private void SpawnExplosionEffect()
    {
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, transform.rotation);
            Destroy(effect, 2f);
        }
    }

    private void ProcessExplosionDamage()
    {
        Collider[] affectedObjects = Physics.OverlapSphere(transform.position, explosionRadius);
        
        foreach (Collider target in affectedObjects)
        {
            ApplyDamageToEnemy(target);
            ApplyPhysicsForce(target);
        }
    }

    private void ApplyDamageToEnemy(Collider target)
    {
        if (target.CompareTag("Enemy"))
        {
            EnemyHealth healthComponent = target.GetComponent<EnemyHealth>();
            if (healthComponent != null)
            {
                healthComponent.TakeDamage(damage);
            }
        }
    }

    private void ApplyPhysicsForce(Collider target)
    {
        Rigidbody targetRigidbody = target.GetComponent<Rigidbody>();
        if (targetRigidbody != null)
        {
            targetRigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}