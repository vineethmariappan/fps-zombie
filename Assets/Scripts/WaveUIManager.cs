using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WaveUIManager : MonoBehaviour
{
    public TMP_Text waveNumberText;
    public TextMesh waveTimerText;
    public TMP_Text waveNotifyText;
    public TMP_Text enemyCountText;

    private int currentWave = 0;
    private float waveTimeRemaining = 0f;
    private Coroutine hideNotificationCoroutine;

    public void SetWaveNumber(int waveNumber)
    {
        UpdateWaveNumber(waveNumber);
    }

    public void SetWaveTimeRemaining(float timeRemaining)
    {
        UpdateWaveTimer(timeRemaining);
    }

    public void SetEnemyCount(int remaining, int total)
    {
        UpdateEnemyCount(remaining, total);
    }

    public void DisplayWaveNotification(string message, float duration = 3f)
    {
        ShowNotification(message, duration);
    }

    public void DisplayFreeRoamTimeRemaining(float timeRemaining)
    {
        ShowFreeRoamTimer(timeRemaining);
    }

    public void DisplayWaveStats(int waveNumber, int enemiesKilled, float timeUsed)
    {
        ShowWaveCompletionStats(waveNumber, enemiesKilled, timeUsed);
    }

    private void UpdateWaveNumber(int waveNumber)
    {
        currentWave = waveNumber;
        if (waveNumberText != null)
        {
            waveNumberText.text = $"Wave: {currentWave}";
        }
    }

    private void UpdateWaveTimer(float timeRemaining)
    {
        waveTimeRemaining = timeRemaining;
        if (waveTimerText != null)
        {
            string formattedTime = FormatTime(waveTimeRemaining);
            waveTimerText.text = $"Time: {formattedTime}";
        }
    }

    private void UpdateEnemyCount(int remaining, int total)
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"Enemies: {remaining}/{total}";
        }
    }

    private void ShowNotification(string message, float duration)
    {
        if (waveNotifyText == null) return;

        CancelPreviousNotification();
        ActivateNotification(message);
        ScheduleNotificationHide(duration);
    }

    private void CancelPreviousNotification()
    {
        if (hideNotificationCoroutine != null)
        {
            StopCoroutine(hideNotificationCoroutine);
        }
    }

    private void ActivateNotification(string message)
    {
        waveNotifyText.text = message;
        waveNotifyText.gameObject.SetActive(true);
    }

    private void ScheduleNotificationHide(float duration)
    {
        hideNotificationCoroutine = StartCoroutine(HideWaveNotificationAfterDelay(duration));
    }

    private IEnumerator HideWaveNotificationAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideWaveNotification();
    }

    private void HideWaveNotification()
    {
        if (waveNotifyText != null)
        {
            waveNotifyText.gameObject.SetActive(false);
        }
    }

    private void ShowFreeRoamTimer(float timeRemaining)
    {
        if (waveNotifyText == null) return;

        waveNotifyText.text = $"Next Wave in: {Mathf.CeilToInt(timeRemaining)}s";
        waveNotifyText.gameObject.SetActive(true);
    }

    private void ShowWaveCompletionStats(int waveNumber, int enemiesKilled, float timeUsed)
    {
        string formattedTime = FormatTime(timeUsed);
        string message = $"Wave {waveNumber} Complete!\n{enemiesKilled} enemies defeated in {formattedTime}";
        DisplayWaveNotification(message, 4f);
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return $"{minutes:00}:{seconds:00}";
    }
}