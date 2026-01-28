using UnityEngine;
using UnityEngine.SceneManagement;
public class SelectScreen : MonoBehaviour
{
    public void NewGame()
    {
        DataPersistenceManager.instance.NewGame();
        DataPersistenceManager.instance.gameData.sceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        DataPersistenceManager.instance.SaveGame();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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

        SceneManager.LoadScene(index);
    }

   
}
