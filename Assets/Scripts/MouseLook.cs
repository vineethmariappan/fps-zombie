using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    [HideInInspector]
    public Transform myCamera;
    public static bool isUIActive = false;

    void Awake()
    {
        InitializeCamera();
    }

    void Update()
    {
        if (isUIActive) return;

        ProcessMouseInput();
        HandleCursorLock();
        UpdateDeltaTime();
        ProcessHeadBob();
    }

    void FixedUpdate()
    {
        UpdateSensitivity();
        ApplyingStuff();
    }

    public static void SetCursorState(bool uiActive)
    {
        if (uiActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    [Header("Z Rotation Camera")]
    [HideInInspector] public float timer;
    [HideInInspector] public int int_timer;
    [HideInInspector] public float zRotation;
    [HideInInspector] public float wantedZ;
    [HideInInspector] public float timeSpeed = 2;
    [HideInInspector] public float timerToRotateZ;

    [Tooltip("Current mouse sensivity, changes in the weapon properties")]
    public float mouseSensitvity = 0;
    [HideInInspector]
    public float mouseSensitvity_notAiming = 300;
    [HideInInspector]
    public float mouseSensitvity_aiming = 50;

    private float rotationYVelocity, cameraXVelocity;
    [Tooltip("Speed that determines how much camera rotation will lag behind mouse movement.")]
    public float yRotationSpeed, xCameraSpeed;

    [HideInInspector]
    public float wantedYRotation;
    [HideInInspector]
    public float currentYRotation;

    [HideInInspector]
    public float wantedCameraXRotation;
    [HideInInspector]
    public float currentCameraXRotation;

    [Tooltip("Top camera angle.")]
    public float topAngleView = 60;
    [Tooltip("Minimum camera angle.")]
    public float bottomAngleView = -45;

    private Vector2 velocityGunFollow;
    private float gunWeightX, gunWeightY;
    [Tooltip("Current weapon that player carries.")]
    [HideInInspector]
    public GameObject weapon;
    private Gun gun;

    float deltaTime = 0.0f;
    [Tooltip("Shows FPS in top left corner.")]
    public bool showFps = true;

    private void InitializeCamera()
    {
        myCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        SetCursorState(isUIActive);
    }

    private void ProcessMouseInput()
    {
        MouseInputMovement();
    }

    private void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void UpdateDeltaTime()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void ProcessHeadBob()
    {
        if (GetComponent<PlayerMovement>().currentSpeed > 1)
        {
            HeadMovement();
        }
    }

    void HeadMovement()
    {
        timer += timeSpeed * Time.deltaTime;
        int_timer = Mathf.RoundToInt(timer);
        
        if (int_timer % 2 == 0)
        {
            wantedZ = -1;
        }
        else
        {
            wantedZ = 1;
        }

        zRotation = Mathf.Lerp(zRotation, wantedZ, Time.deltaTime * timerToRotateZ);
    }

    private void UpdateSensitivity()
    {
        if (Input.GetAxis("Fire2") != 0)
        {
            mouseSensitvity = mouseSensitvity_aiming;
        }
        else if (GetComponent<PlayerMovement>().maxSpeed > 5)
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }
        else
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }
    }

    void MouseInputMovement()
    {
        wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;
        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;
        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
    }

    void ApplyingStuff()
    {
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

        WeaponRotation();

        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        myCamera.localRotation = Quaternion.Euler(currentCameraXRotation, 0, zRotation);
    }

    void WeaponRotation()
    {
        if (!weapon)
        {
            weapon = GameObject.FindGameObjectWithTag("Weapon");
            
            if (weapon)
            {
                if (weapon.GetComponent<Gun>())
                {
                    try
                    {
                        gun = GameObject.FindGameObjectWithTag("Weapon").GetComponent<Gun>();
                    }
                    catch (System.Exception ex)
                    {
                    }
                }
            }
        }
    }

    void OnGUI()
    {
        if (showFps)
        {
            FPSCounter();
        }
    }

    void FPSCounter()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}