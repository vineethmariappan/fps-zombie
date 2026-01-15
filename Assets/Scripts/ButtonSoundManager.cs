using UnityEngine;

public class ButtonSoundManager : MonoBehaviour
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private AudioSource soundPlayer;

    void Start()
    {
        InitializeSoundPlayer();
    }

    public void PlayHoverSound()
    {
        PlaySoundEffect(hoverSound);
    }

    public void PlayClickSound()
    {
        PlaySoundEffect(clickSound);
    }

    private void InitializeSoundPlayer()
    {
        soundPlayer = GetComponent<AudioSource>();
        
        if (soundPlayer == null)
        {
            soundPlayer = gameObject.AddComponent<AudioSource>();
        }
    }

    private void PlaySoundEffect(AudioClip clip)
    {
        if (clip != null && soundPlayer != null)
        {
            soundPlayer.PlayOneShot(clip);
        }
    }
}