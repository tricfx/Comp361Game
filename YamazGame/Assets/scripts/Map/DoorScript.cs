using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] string targetScene;
    [SerializeField] Vector2 spawnPosition;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitbox")) return;
        DataPersistenceManager.instance.SaveGame();
        PlayerSpawn.nextSpawn = spawnPosition;
        // DataPersistenceManager manager = FindFirstObjectByType<DataPersistenceManager>();
        // manager.SaveGame();
         if(targetScene == ""){
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else {
            SceneManager.LoadScene(targetScene);
        }
    }
}