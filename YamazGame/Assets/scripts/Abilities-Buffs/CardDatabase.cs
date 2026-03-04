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
    public Card GetCardByID(string id)
    {
        if (string.IsNullOrEmpty(id) || allRewards == null) return null;

        for (int i = 0; i < allRewards.Length; i++)
        {
            var c = allRewards[i];
            if (c != null && c.cardID == id)
                return c;
        }
        return null;
    }
}