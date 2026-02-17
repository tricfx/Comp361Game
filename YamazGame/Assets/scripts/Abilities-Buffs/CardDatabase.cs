using UnityEngine;

[CreateAssetMenu(menuName = "Rewards/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    //i think we should populate this here rather than fetching from database
    public Card[] allRewards;

    public Card GetRandomReward()
    {
        return allRewards[Random.Range(0, allRewards.Length)];
    }
}