using UnityEngine;
using System;

public class GemManager : MonoBehaviour, IDataPersistence
{
    public static GemManager Instance;

    public event Action<int> OnGemsChanged;

    [SerializeField] private int startingGems = 100;

    private int currentGems;

    public int CurrentGems => currentGems;

    private void Awake()
    {
        currentGems = startingGems;

        if (DataPersistenceManager.instance != null)
        {
            DataPersistenceManager.instance.LoadGame();
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    

    public void AddGems(int amount)
    {
        if (amount <= 0) return;

        currentGems += amount;
        OnGemsChanged?.Invoke(currentGems);
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0 || currentGems < amount)
            return false;

        currentGems -= amount;
        OnGemsChanged?.Invoke(currentGems);
        return true;
    }


    public void SaveData(ref GameData data)
    {
        Debug.Log("Saving gems: " + currentGems);
        data.gemsCollected = currentGems;
        // We will also save the current scene index to know where to load back in from here since why not
        data.sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }


    public void LoadData(GameData data)
    {
        Debug.Log("Loading gems: " + data.gemsCollected);
        this.currentGems = data.gemsCollected;
        OnGemsChanged?.Invoke(currentGems);
    }
}