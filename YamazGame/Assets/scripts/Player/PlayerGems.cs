using UnityEngine;

public class PlayerGems : MonoBehaviour
{
    [Header("Gems")]
    [SerializeField] private int startingGems = 0;
    private int currentGems;

    private void Awake()
    {
        currentGems = startingGems;
    }

    public int GetGems()
    {
        return currentGems;
    }

    public void AddGems(int amount)
    {
        if (amount <= 0) return;

        currentGems += amount;
        Debug.Log($"Gems added: {amount}. Total gems: {currentGems}");
    }

    public bool SpendGems(int amount)
    {
        if (amount <= 0) return true;

        if (currentGems < amount)
        {
            Debug.Log("Not enough gems!");
            return false;
        }

        currentGems -= amount;
        Debug.Log($"Gems spent: {amount}. Remaining gems: {currentGems}");
        return true;
    }

    public void SetGems(int amount)
    {
        currentGems = Mathf.Max(0, amount);
        Debug.Log($"Gems set to: {currentGems}");
    }
}
