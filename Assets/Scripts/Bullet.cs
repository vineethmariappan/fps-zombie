using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float maxDistance = 1000f;
    public GameObject decalHitWall;
    public GameObject bloodEffect;
    public float floatInFrontOfWall = 0.05f;
    public float bulletDamage = 10f;
    public LayerMask ignoreLayer;
    public float bulletSpeed = 50f;

    private RaycastHit hitInfo;
    private Rigidbody rigidBody;
    private float projectileLifetime = 5f;

    void Start()
    {
        InitializeProjectile();
    }

    void Update()
    {
        if (MouseLook.isUIActive || rigidBody == null) return;

        if (rigidBody.velocity.sqrMagnitude > 0)
        {
            CheckCollision();
        }
    }

    private void InitializeProjectile()
    {
        rigidBody = GetComponent<Rigidbody>();
        
        if (rigidBody != null)
        {
            rigidBody.velocity = transform.forward * bulletSpeed;
        }

        Destroy(gameObject, projectileLifetime);
    }

    private void CheckCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDistance, ~ignoreLayer))
        {
            ProcessImpact();
        }
    }

    private void ProcessImpact()
    {
        if (hitInfo.transform.CompareTag("LevelPart"))
        {
            SpawnWallDecal();
        }
        else if (hitInfo.transform.CompareTag("Enemy"))
        {
            ProcessEnemyHit();
        }

        Destroy(gameObject);
    }

    private void SpawnWallDecal()
    {
        if (decalHitWall != null)
        {
            Vector3 spawnPosition = hitInfo.point + hitInfo.normal * floatInFrontOfWall;
            Quaternion spawnRotation = Quaternion.LookRotation(hitInfo.normal);
            Instantiate(decalHitWall, spawnPosition, spawnRotation);
        }
    }

    private void ProcessEnemyHit()
    {
        if (bloodEffect != null)
        {
            Instantiate(bloodEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        }

        EnemyHealth targetHealth = hitInfo.transform.GetComponent<EnemyHealth>();
        if (targetHealth != null)
        {
            targetHealth.TakeDamage(bulletDamage);
        }
    }
}