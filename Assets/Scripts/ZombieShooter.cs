using UnityEngine;

public class ZombieShooter : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    [Header("Shooting Settings")]
    public float shootRange = 15f;
    public float shootCooldown = 2f;
    public float projectileSpeed = 5f;
    public float shootDamage = 5f;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float preferredDistance = 10f;
    public float avoidanceRadius = 1f;
    public float avoidanceForce = 2f;

    private float lastShootTime;
    private Rigidbody rb;

    void Awake()
    {
        InitializeShooter();
    }

    void Start()
    {
        ConfigureComponents();
    }

    void FixedUpdate()
    {
        ProcessMovement();
    }

    void Update()
    {
        ProcessShooting();
    }

    void OnDrawGizmosSelected()
    {
        DrawDebugGizmos();
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void InitializeShooter()
    {
    }

    private void ConfigureComponents()
    {
        rb = GetComponent<Rigidbody>();
        lastShootTime = -shootCooldown;
    }

    private void ProcessMovement()
    {
        if (player == null) return;

        float distanceToPlayer = CalculateDistanceToPlayer();
        Vector3 direction = CalculateDirectionToPlayer();

        RotateTowardsPlayer(direction);
        ExecutePositioning(distanceToPlayer, direction);
        AvoidOtherEnemies();
    }

    private void ProcessShooting()
    {
        if (player == null) return;

        float distanceToPlayer = CalculateDistanceToPlayer();

        if (CanShoot(distanceToPlayer))
        {
            Shoot();
        }
    }

    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    private Vector3 CalculateDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }

    private void ExecutePositioning(float distance, Vector3 direction)
    {
        if (distance > shootRange)
        {
            MoveCloser(direction);
        }
        else if (distance < preferredDistance)
        {
            MoveAway(direction);
        }
    }

    private void MoveCloser(Vector3 direction)
    {
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
    }

    private void MoveAway(Vector3 direction)
    {
        rb.MovePosition(transform.position - direction * speed * 0.5f * Time.fixedDeltaTime);
    }

    private bool CanShoot(float distance)
    {
        return distance <= shootRange && Time.time >= lastShootTime + shootCooldown;
    }

    private void Shoot()
    {
        if (projectilePrefab == null)
        {
            return;
        }

        Vector3 spawnPos = CalculateSpawnPosition();
        Vector3 shootDirection = CalculateShootDirection(spawnPos);

        spawnPos += shootDirection * 2f;

        GameObject projectile = CreateProjectile(spawnPos, shootDirection);
        InitializeProjectile(projectile, shootDirection);

        lastShootTime = Time.time;
    }

    private Vector3 CalculateSpawnPosition()
    {
        return shootPoint != null ? shootPoint.position : transform.position + Vector3.up * 1.5f;
    }

    private Vector3 CalculateShootDirection(Vector3 spawnPos)
    {
        return (player.position - spawnPos).normalized;
    }

    private GameObject CreateProjectile(Vector3 position, Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction);
        return Instantiate(projectilePrefab, position, rotation);
    }

    private void InitializeProjectile(GameObject projectile, Vector3 direction)
    {
        ZombieProjectile projScript = projectile.GetComponent<ZombieProjectile>();
        
        if (projScript != null)
        {
            projScript.Initialize(direction, projectileSpeed, shootDamage);
        }
        else
        {
            Destroy(projectile);
        }
    }

    private void RotateTowardsPlayer(Vector3 direction)
    {
        direction.y = 0;
        
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 5f));
        }
    }

    private void AvoidOtherEnemies()
    {
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, avoidanceRadius);
        
        foreach (Collider enemy in nearbyEnemies)
        {
            if (enemy.gameObject != this.gameObject)
            {
                ApplyAvoidanceForce(enemy);
            }
        }
    }

    private void ApplyAvoidanceForce(Collider enemy)
    {
        Vector3 avoidanceDir = transform.position - enemy.transform.position;
        avoidanceDir.y = 0;
        rb.AddForce(avoidanceDir.normalized * avoidanceForce, ForceMode.Force);
    }

    private void DrawDebugGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
}