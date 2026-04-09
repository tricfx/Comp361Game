using UnityEngine;

public class RewardEngine : MonoBehaviour
{
    [SerializeField] private float difficultyMod = 1.006f;
    [SerializeField] private int floorBonusBase = 1;

    public int Calculate(int floor, int baseKills)
    {
        int floorBonus = Mathf.RoundToInt(floor * floorBonusBase * 0.7f);
        int total = Mathf.RoundToInt((baseKills * difficultyMod) + floorBonus);
        return total;
    }
}