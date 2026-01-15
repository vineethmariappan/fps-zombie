using UnityEngine;

public class ZombieWalkAI : MonoBehaviour
{
    public Transform player;
    public float speed = 1f;
    public float stoppingDistance = 1f;
    public float avoidanceRadius = 1f;
    public float avoidanceForce = 2f;
    public float attackDamage = 10f;
    public float attackCooldown = 1f;

    private float lastAttackTime;
    private Rigidbody rb;
    private Animator animator;

    void Start()
    {
        InitializeComponents();
    }

    void FixedUpdate()
    {
        ProcessAI();
    }

    void OnDrawGizmosSelected()
    {
        DrawDebugGizmos();
    }

    public void SetPlayer(Transform playerTransform)
    {
        player = playerTransform;
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void ProcessAI()
    {
        if (player == null) return;

        float distanceToPlayer = CalculateDistanceToPlayer();

        if (distanceToPlayer > stoppingDistance)
        {
            ExecuteMovement();
        }
        else
        {
            ExecuteAttack();
        }

        AvoidOtherEnemies();
    }

    private float CalculateDistanceToPlayer()
    {
        return Vector3.Distance(transform.position, player.position);
    }

    private void ExecuteMovement()
    {
        Vector3 direction = CalculateDirectionToPlayer();
        MoveTowardsPlayer(direction);
        RotateTowardsPlayer(direction);
        UpdateMovementAnimations();
    }

    private Vector3 CalculateDirectionToPlayer()
    {
        return (player.position - transform.position).normalized;
    }

    private void MoveTowardsPlayer(Vector3 direction)
    {
        rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
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

    private void UpdateMovementAnimations()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isAttacking", false);
        }
    }

    private void ExecuteAttack()
    {
        AttackPlayer();
        Vector3 direction = CalculateDirectionToPlayer();
        RotateTowardsPlayer(direction);
        StopWalkingAnimation();
    }

    private void StopWalkingAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
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

    private void AttackPlayer()
    {
        float attackRange = stoppingDistance + 0.2f;
        float distanceToPlayer = CalculateDistanceToPlayer();

        if (IsWithinAttackRange(distanceToPlayer, attackRange) && CanAttack())
        {
            PerformAttack();
        }
        else if (distanceToPlayer > attackRange)
        {
            StopAttackAnimation();
        }
    }

    private bool IsWithinAttackRange(float distance, float range)
    {
        return distance <= range;
    }

    private bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    private void PerformAttack()
    {
        TriggerAttackAnimation();
        ApplyDamageToPlayer();
        lastAttackTime = Time.time;
    }

    private void TriggerAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    private void StopAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    private void ApplyDamageToPlayer()
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    private void DrawDebugGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance + 0.2f);
    }
}