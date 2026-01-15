using UnityEngine;
using TMPro;

public class GameOverDisplay : MonoBehaviour
{
    public TextMeshProUGUI gameOverText;

    void Start()
    {
        DisplaySurvivalStats();
    }

    private void DisplaySurvivalStats()
    {
        int survivedWaves = RetrieveWaveCount();
        UpdateDisplayText(survivedWaves);
    }

    private int RetrieveWaveCount()
    {
        return PlayerPrefs.GetInt("WavesSurvived", 0);
    }

    private void UpdateDisplayText(int waveCount)
    {
        gameOverText.text = $"You Survived {waveCount} Waves";
    }
}