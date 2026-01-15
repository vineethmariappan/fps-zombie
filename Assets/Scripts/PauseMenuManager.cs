using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    private Gun gunScript;
    private GunInventory gunInventory;

    void Start()
    {
        InitializeReferences();
    }

    void Update()
    {
        HandlePauseInput();
    }

    public void PauseGame()
    {
        ActivatePauseState();
    }

    public void ResumeGame()
    {
        DeactivatePauseState();
    }

    public void LoadMainMenu()
    {
        NavigateToMainMenu();
    }

    public void QuitGame()
    {
        ExitApplication();
    }

    private void InitializeReferences()
    {
        gunInventory = FindObjectOfType<GunInventory>();
    }

    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void ActivatePauseState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        isPaused = true;
        MouseLook.isUIActive = true;
        Time.timeScale = 0f;

        pauseMenu.SetActive(true);

        EnsureGunScriptReference();
        HideGameplayUI();
    }

    private void DeactivatePauseState()
    {
        isPaused = false;
        MouseLook.isUIActive = false;
        MouseLook.SetCursorState(false);
        Time.timeScale = 1f;

        pauseMenu.SetActive(false);

        EnsureGunScriptReference();
        ShowGameplayUI();
    }

    private void EnsureGunScriptReference()
    {
        if (gunScript == null)
        {
            gunScript = FindObjectOfType<Gun>();
        }
    }

    private void HideGameplayUI()
    {
        if (gunInventory != null)
        {
            gunInventory.isInventoryVisible = false;
        }

        if (gunScript != null)
        {
            gunScript.isCrosshairVisible = false;
        }
    }

    private void ShowGameplayUI()
    {
        if (gunInventory != null)
        {
            gunInventory.isInventoryVisible = true;
        }

        if (gunScript != null)
        {
            gunScript.isCrosshairVisible = true;
        }
    }

    private void NavigateToMainMenu()
    {
        Time.timeScale = 1f;
        MouseLook.isUIActive = true;
        MouseLook.SetCursorState(true);
        SceneManager.LoadScene("MainMenu");
    }

    private void ExitApplication()
    {
        Application.Quit();
    }
}