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
        GameData data = dataHandler.Load();
        if (data == null)
        {
            StartCoroutine(BackendManager.Instance.GetPlayerState()); // this already handles the case where they don't have any data -> initializing default values
        }

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        dataHandler.Save(gameData);
        
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
    private void OnApplicationQuit()
    {
        SaveGame();
    }
    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects =
    Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None)
          .OfType<IDataPersistence>();

        return new List<IDataPersistence>(dataPersistenceObjects);
    }   

}
