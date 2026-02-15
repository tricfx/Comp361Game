using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] Vector2 spawnPosition;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        SceneManager.LoadScene(targetScene);
        PlayerSpawn.nextSpawn = spawnPosition;
    }
}