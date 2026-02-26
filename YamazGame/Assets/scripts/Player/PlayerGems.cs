using UnityEngine;

// Moved content to GemManager
public class PlayerGems : MonoBehaviour
{
    public int CurrentGems => GemManager.Instance.CurrentGems;

    public void RewardGems(int amount)
    {
        GemManager.Instance.AddGems(amount);
    }
    

    public bool TrySpendGems(int amount)
    {
        return GemManager.Instance.SpendGems(amount);
    }
}