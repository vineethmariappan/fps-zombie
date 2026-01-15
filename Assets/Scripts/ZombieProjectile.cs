using UnityEngine;
using System.Collections;

public class ZombieProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float maxDistance = 1000f;
    public float projectileSpeed = 20f;
    public float damage = 15f;
    public float lifetime = 5f;

    [Header("Effects")]
    public GameObject hitEffect;

    private Rigidbody rb;
    private bool hasInitialized = false;
    private float spawnTime;
    private int frameCount = 0;
    private string destructionReason = "Unknown";
    private Vector3 spawnPosition;

    void Awake()
    {
        InitializeProjectile();
    }

    void Start()
    {
        ScheduleAutoDestruction();
    }

    void Update()
    {
        TrackProjectileState();
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessCollision(other);
    }

    void OnDestroy()
    {
        LogDestructionData();
    }

    public void Initialize(Vector3 shootDirection, float speed, float projectileDamage)
    {
        ConfigureProjectile(shootDirection, speed, projectileDamage);
    }

    private void InitializeProjectile()
    {
        spawnTime = Time.time;
        spawnPosition = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void ScheduleAutoDestruction()
    {
        Destroy(gameObject, lifetime);
    }

    private void TrackProjectileState()
    {
        frameCount++;

        if (!hasInitialized)
        {
            return;
        }

        if (rb == null)
        {
            return;
        }
    }

    private void ConfigureProjectile(Vector3 shootDirection, float speed, float projectileDamage)
    {
        damage = projectileDamage;
        projectileSpeed = speed;
        hasInitialized = true;

        if (rb != null)
        {
            ApplyVelocity(shootDirection);
            StartCoroutine(CheckVelocityAfterFrame());
        }

        if (shootDirection != Vector3.zero)
        {
            SetRotation(shootDirection);
        }
    }

    private void ApplyVelocity(Vector3 direction)
    {
        rb.velocity = direction.normalized * projectileSpeed;
    }

    private void SetRotation(Vector3 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction);
    }

    private IEnumerator CheckVelocityAfterFrame()
    {
        yield return null;
        
        if (rb != null && rb.velocity.magnitude < 1f)
        {
        }
    }

    private void ProcessCollision(Collider other)
    {
        if (ShouldIgnoreCollision(other))
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            HandlePlayerHit(other);
            return;
        }

        HandleEnvironmentHit(other);
    }

    private bool ShouldIgnoreCollision(Collider other)
    {
        return other.gameObject.layer == LayerMask.NameToLayer("Enemy") ||
               other.gameObject.layer == LayerMask.NameToLayer("Projectile") ||
               other.CompareTag("Projectile");
    }

    private void HandlePlayerHit(Collider player)
    {
        SpawnHitEffect();
        ApplyDamageToPlayer(player);
        destructionReason = "Hit Player";
        Destroy(gameObject);
    }

    private void HandleEnvironmentHit(Collider environment)
    {
        SpawnHitEffect();
        destructionReason = $"Hit {environment.name} (Tag: {environment.tag})";
        Destroy(gameObject);
    }

    private void SpawnHitEffect()
    {
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }
    }

    private void ApplyDamageToPlayer(Collider player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void LogDestructionData()
    {
        float aliveTime = Time.time - spawnTime;
        float distanceTraveled = Vector3.Distance(transform.position, spawnPosition);
    }
}