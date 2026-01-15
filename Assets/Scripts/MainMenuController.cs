using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        InitializeMenuState();
    }

    public void StartGame()
    {
        LoadGameScene();
    }

    public void ExitGame()
    {
        QuitApplication();
    }

    private void InitializeMenuState()
    {
        MouseLook.isUIActive = true;
        MouseLook.SetCursorState(true);
    }

    private void LoadGameScene()
    {
        MouseLook.isUIActive = false;
        MouseLook.SetCursorState(false);
        SceneManager.LoadScene("MainScene");
    }

    private void QuitApplication()
    {
        Application.Quit();
    }
}