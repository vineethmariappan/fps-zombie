using UnityEngine;
using TMPro;

public class UIController : MonoBehaviour
{
    public TMP_Text waveText;

    public void UpdateWaveText(int waveNumber, float countdownTime)
    {
        string timeFormatted = FormatTime(countdownTime);
        waveText.text = $"Wave {waveNumber} - {timeFormatted}";
    }

    private string FormatTime(float time)
    {
        int minutes = CalculateMinutes(time);
        int seconds = CalculateSeconds(time);
        int milliseconds = CalculateMilliseconds(time);
        
        return string.Format("{00:00}:{01:00}:{02:00}", minutes, seconds, milliseconds);
    }

    private int CalculateMinutes(float time)
    {
        return Mathf.FloorToInt(time / 60);
    }

    private int CalculateSeconds(float time)
    {
        return Mathf.FloorToInt(time % 60);
    }

    private int CalculateMilliseconds(float time)
    {
        return Mathf.FloorToInt((time * 100) % 100);
    }
}