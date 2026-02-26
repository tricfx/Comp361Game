using UnityEngine;
using System;

public class GemManager : MonoBehaviour
{
    public static GemManager Instance; // Only one GemManager exist in the game

    public event Action<int> OnGemsChanged;

    [SerializeField] private int startingGems = 0;

    private int currentGems;

    public int CurrentGems => currentGems;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentGems = startingGems;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;

        currentGems += amount;
        OnGemsChanged?.Invoke(currentGems);

        Debug.Log("Gems Added: " + amount);
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0) return false;

        if (currentGems < amount)
        {
            Debug.Log("Not enough gems");
            return false;
        }

        currentGems -= amount;
        OnGemsChanged?.Invoke(currentGems);

        Debug.Log("Gems Spent: " + amount);
        return true;
    }
}