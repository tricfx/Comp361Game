using UnityEngine;
using System.Linq;
using System.Collections.Generic;
public class DataPersistenceManager : MonoBehaviour
{

    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [SerializeField] private bool useEncryption;

    public GameData gameData;

    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("More than one Data Persistence Manager in the scene.");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.LogError("More than one Data Persistence Manager in the scene.");
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, fileName,useEncryption);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    

    public void NewGame()
    {
        this.gameData = new GameData();
    }
    public void LoadGame()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();

        // Try load from file
        this.gameData = dataHandler.Load();

        // If no file exists, create defaults instead of crashing
        if (this.gameData == null)
        {
            this.gameData = new GameData();
             dataHandler.Save(this.gameData);
        }        

        if (BackendManager.Instance.SessionManager.AccessToken != null)
        {

            StartCoroutine(BackendManager.Instance.GetPlayerState(
                state =>
                {
                    gameData = new GameData
                    {
                        sceneIndex = state.scene_number,
                        gemsCollected = state.gems_amount,
                        abilities = state.abilities,
                        left_during_combat = state.left_during_combat,
                        buffs = state.buffs
                    };
                    dataHandler.Save(gameData);
                }
            ));
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData); // now always non-null
        }
    }

    public void SaveGame()
    {
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
        if (BackendManager.Instance.SessionManager.AccessToken != null)
        {
            PlayerStateRequest playerState = new PlayerStateRequest
            {
                new_scene_number = gameData.sceneIndex,
                new_gems_amount = gameData.gemsCollected,
                new_abilities = gameData.abilities,
                new_left_during_combat = gameData.left_during_combat,
                new_buffs = gameData.buffs
            };
            StartCoroutine(BackendManager.Instance.UpdatePlayerState(playerState));
            
        }
        
    }
    private void OnApplicationQuit()
    {
        SaveGame();
        if (BackendManager.Instance.SessionManager.AccessToken != null)
        {
            StartCoroutine(BackendManager.Instance.SignOut(
            () => Debug.Log("Signed out successfully on application quit")
            ));
        }
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =
    Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
          .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }   

}
