using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectScreen : MonoBehaviour
{
    [SerializeField]
    private LevelLoader levelLoader;
    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        DataPersistenceManager.instance.SaveGame();

        levelLoader.LoadLevel(DataPersistenceManager.instance.gameData.sceneIndex);
    }
    public void LoadGame()
    {
        DataPersistenceManager.instance.LoadGame();
        int index = DataPersistenceManager.instance.gameData.sceneIndex;

        if (index == 0)
        {
            NewGame();
            return;
        }

        levelLoader.LoadLevel(index);
    }

   
}
