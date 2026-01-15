using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;

    public float currentSpeed;
    [HideInInspector] public Transform cameraMain;
    public float jumpForce = 500;
    [HideInInspector] public Vector3 cameraPosition;
    public bool isSpeedBoosted = false;

    [Tooltip("The maximum speed you want to achieve")]
    public int maxSpeed = 5;
    [Tooltip("The higher the number the faster it will stop")]
    public float deaccelerationSpeed = 15.0f;
    [Tooltip("Force that is applied when moving forward or backward")]
    public float accelerationSpeed = 50000.0f;
    [Tooltip("Tells us weather the player is grounded or not.")]
    public bool grounded;

    [Tooltip("Put 'Player' layer here")]
    [Header("Shooting Properties")]
    private LayerMask ignoreLayer;
    [Tooltip("Put BulletSpawn gameobject here, palce from where bullets are created.")]
    [HideInInspector]
    public Transform bulletSpawn;

    [Header("BloodForMelleAttaacks")]
    RaycastHit hit;
    public GameObject bloodEffect;

    [Header("Player SOUNDS")]
    public AudioSource _jumpSound;
    public AudioSource _freakingZombiesSound;
    public AudioSource _hitSound;
    public AudioSource _walkSound;
    public AudioSource _runSound;
    public AudioSource _hurtSound;

    private Vector3 slowdownV;
    private Vector2 horizontalMovement;
    RaycastHit hitInfo;
    private float meleeAttack_cooldown;
    private string currentWeapo;
    Ray ray1, ray2, ray3, ray4, ray5, ray6, ray7, ray8, ray9;
    private float rayDetectorMeeleSpace = 0.15f;
    private float offsetStart = 0.05f;
    public bool been_to_meele_anim = false;
    private GameObject myBloodEffect;

    void Awake()
    {
        InitializeComponents();
    }

    void Update()
    {
        if (MouseLook.isUIActive) return;
        
        ProcessJump();
        ProcessCrouch();
        UpdateMovementAudio();
    }

    void FixedUpdate()
    {
        if (MouseLook.isUIActive) return;

        ProcessMeleeRaycast();
        ExecuteMovement();
    }

    void OnCollisionStay(Collision other)
    {
        CheckGroundContact(other);
    }

    void OnCollisionExit()
    {
        grounded = false;
    }

    public void PlayHurtSound()
    {
        if (_hurtSound != null)
        {
            _hurtSound.Play();
        }
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        cameraMain = transform.Find("Main Camera").transform;
        bulletSpawn = cameraMain.Find("BulletSpawn").transform;
        ignoreLayer = 1 << LayerMask.NameToLayer("Player");
    }

    private void ExecuteMovement()
    {
        UpdateCurrentSpeed();
        ClampHorizontalVelocity();
        ApplyVelocity();
        ApplyDeceleration();
        ApplyMovementForce();
        UpdateDecelerationSpeed();
    }

    private void UpdateCurrentSpeed()
    {
        currentSpeed = rb.velocity.magnitude;
        horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);
    }

    private void ClampHorizontalVelocity()
    {
        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }
    }

    private void ApplyVelocity()
    {
        rb.velocity = new Vector3(
            horizontalMovement.x,
            rb.velocity.y,
            horizontalMovement.y
        );
    }

    private void ApplyDeceleration()
    {
        if (grounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity,
                new Vector3(0, rb.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed);
        }
    }

    private void ApplyMovementForce()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        float forceMultiplier = grounded ? accelerationSpeed : accelerationSpeed / 2;

        rb.AddRelativeForce(
            horizontalInput * forceMultiplier * Time.deltaTime,
            0,
            verticalInput * forceMultiplier * Time.deltaTime
        );
    }

    private void UpdateDecelerationSpeed()
    {
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }

    private void ProcessJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            ExecuteJump();
        }
    }

    private void ExecuteJump()
    {
        rb.AddRelativeForce(Vector3.up * jumpForce);
        
        if (_jumpSound)
        {
            _jumpSound.Play();
        }
        
        _walkSound.Stop();
        _runSound.Stop();
    }

    private void UpdateMovementAudio()
    {
        if (_walkSound && _runSound)
        {
            if (RayCastGrounded())
            {
                if (currentSpeed > 1)
                {
                    PlayAppropriateMovementSound();
                }
                else
                {
                    StopMovementSounds();
                }
            }
            else
            {
                StopMovementSounds();
            }
        }
    }

    private void PlayAppropriateMovementSound()
    {
        if (maxSpeed == 3)
        {
            if (!_walkSound.isPlaying)
            {
                _walkSound.Play();
                _runSound.Stop();
            }
        }
        else if (maxSpeed > 4)
        {
            if (!_runSound.isPlaying)
            {
                _walkSound.Stop();
                _runSound.Play();
            }
        }
    }

    private void StopMovementSounds()
    {
        _walkSound.Stop();
        _runSound.Stop();
    }

    private bool RayCastGrounded()
    {
        RaycastHit groundedInfo;
        
        if (Physics.Raycast(transform.position, transform.up * -1f, out groundedInfo, 1, ~ignoreLayer))
        {
            Debug.DrawRay(transform.position, transform.up * -1f, Color.red, 0.0f);
            return groundedInfo.transform != null;
        }

        return false;
    }

    private void ProcessCrouch()
    {
        if (Input.GetKey(KeyCode.C))
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 0.6f, 1), Time.deltaTime * 15);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * 15);
        }
    }

    private void CheckGroundContact(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            if (Vector2.Angle(contact.normal, Vector3.up) < 60)
            {
                grounded = true;
            }
        }
    }

    private void ProcessMeleeRaycast()
    {
        UpdateMeleeCooldown();
        UpdateCurrentWeapon();
        ConstructMeleeRays();
        DrawMeleeDebugRays();
        CheckMeleeAnimation();
    }

    private void UpdateMeleeCooldown()
    {
        if (meleeAttack_cooldown > -5)
        {
            meleeAttack_cooldown -= 1 * Time.deltaTime;
        }
    }

    private void UpdateCurrentWeapon()
    {
        if (GetComponent<GunInventory>().currentGun)
        {
            if (GetComponent<GunInventory>().currentGun.GetComponent<Gun>())
            {
                currentWeapo = "gun";
            }
        }
    }

    private void ConstructMeleeRays()
    {
        ray1 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace));
        ray2 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace));
        ray3 = new Ray(bulletSpawn.position, bulletSpawn.forward);
        
        ray4 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray5 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) + (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) + (bulletSpawn.up * rayDetectorMeeleSpace));
        ray6 = new Ray(bulletSpawn.position + (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.up * rayDetectorMeeleSpace));
        
        ray7 = new Ray(bulletSpawn.position + (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward + (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray8 = new Ray(bulletSpawn.position - (bulletSpawn.right * offsetStart) - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.right * rayDetectorMeeleSpace) - (bulletSpawn.up * rayDetectorMeeleSpace));
        ray9 = new Ray(bulletSpawn.position - (bulletSpawn.up * offsetStart), bulletSpawn.forward - (bulletSpawn.up * rayDetectorMeeleSpace));
    }

    private void DrawMeleeDebugRays()
    {
        Debug.DrawRay(ray1.origin, ray1.direction, Color.cyan);
        Debug.DrawRay(ray2.origin, ray2.direction, Color.cyan);
        Debug.DrawRay(ray3.origin, ray3.direction, Color.cyan);
        Debug.DrawRay(ray4.origin, ray4.direction, Color.red);
        Debug.DrawRay(ray5.origin, ray5.direction, Color.red);
        Debug.DrawRay(ray6.origin, ray6.direction, Color.red);
        Debug.DrawRay(ray7.origin, ray7.direction, Color.yellow);
        Debug.DrawRay(ray8.origin, ray8.direction, Color.yellow);
        Debug.DrawRay(ray9.origin, ray9.direction, Color.yellow);
    }

    private void CheckMeleeAnimation()
    {
        if (GetComponent<GunInventory>().currentGun)
        {
            bool isMeleeAttacking = GetComponent<GunInventory>().currentGun.GetComponent<Gun>().meeleAttack;
            
            if (!isMeleeAttacking)
            {
                been_to_meele_anim = false;
            }
            
            if (isMeleeAttacking && !been_to_meele_anim)
            {
                been_to_meele_anim = true;
                StartCoroutine("MeeleAttackWeaponHit");
            }
        }
    }

    IEnumerator MeeleAttackWeaponHit()
    {
        if (CheckAnyMeleeRayHit())
        {
            if (hitInfo.transform.tag == "Enemy")
            {
                ProcessEnemyMeleeHit();
            }
        }
        yield return new WaitForEndOfFrame();
    }

    private bool CheckAnyMeleeRayHit()
    {
        return Physics.Raycast(ray1, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray2, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray3, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray4, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray5, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray6, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray7, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray8, out hitInfo, 2f, ~ignoreLayer) ||
               Physics.Raycast(ray9, out hitInfo, 2f, ~ignoreLayer);
    }

    private void ProcessEnemyMeleeHit()
    {
        Transform _other = hitInfo.transform.root.transform;
        
        if (_other.transform.tag == "Enemy")
        {
        }
        
        SpawnBloodEffect(hitInfo, false);
        ApplyMeleeDamage();
    }

    private void ApplyMeleeDamage()
    {
        EnemyHealth enemyHealth = hitInfo.transform.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(150);
        }
    }

    void SpawnBloodEffect(RaycastHit _hitPos, bool swordHitWithGunOrNot)
    {
        if (currentWeapo == "gun")
        {
            Gun.HitMarkerSound();

            if (_hitSound)
            {
                _hitSound.Play();
            }

            if (!swordHitWithGunOrNot)
            {
                if (bloodEffect)
                {
                    Instantiate(bloodEffect, _hitPos.point, Quaternion.identity);
                }
            }
        }
    }
}