using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GameOverManager : MonoBehaviour
{
    void Start()
    {
        InitializeGameOverState();
    }

    public void RestartGame()
    {
        LoadGameScene("MainScene");
    }

    public void GoToMainmanu()
    {
        LoadGameScene("MainMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void InitializeGameOverState()
    {
        MouseLook.isUIActive = true;
        MouseLook.SetCursorState(true);
        Time.timeScale = 1f;
    }

    private void LoadGameScene(string sceneName)
    {
        MouseLook.isUIActive = false;
        MouseLook.SetCursorState(false);
        SceneManager.LoadScene(sceneName);
    }
}