using UnityEngine;

public class Level : MonoBehaviour
{
    private SceneManagerGameplay sceneManager;

    void Awake()
    {
        sceneManager = FindFirstObjectByType<SceneManagerGameplay>();
    }

    public void EnemyKilled()
    {
        sceneManager.RegisterKill();
    }

    public void ClearRoom()
    {
        sceneManager.RoomCleared();
    }

    void Update() 
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            EnemyKilled();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearRoom();
        }    
    }
}