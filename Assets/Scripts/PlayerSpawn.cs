using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public GameObject player;
    public GameObject platform;
    public Transform spawnPoint;

    void Start()
    {
        InitializePlayer();
    }

    private void InitializePlayer()
    {
        SpawnPlayer();
        SchedulePlatformDestruction();
    }

    private void SpawnPlayer()
    {
        Instantiate(player, spawnPoint.position, spawnPoint.rotation);
    }

    private void SchedulePlatformDestruction()
    {
        Destroy(platform, 3f);
    }
}