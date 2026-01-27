using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
public class DataPersistenceManager : MonoBehaviour
{
  public static DataPersistenceManager instance { get; private set; }
    
    private gameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Data Persistence Manager in the scene.");
           
        }
        instance = this;
        
    }
    private void Start()
    {   
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }
    private void nApplicationQuit()
    {
        SaveGame();
    }

    public void NewGame()
    {
        this.gameData = new gameData();
    }

    public void SaveGame()
    {
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }
        Debug.Log("Save Game");
    }
    public void LoadGame()
    {
        if(this.gameData == null)
        {
            Debug.Log("No data found. Initializing data to defaults.");
            NewGame();
        }
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
            .OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }   
}
