using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Buff")]
public class BuffCard : Card
{
    public string buffID;
    public int bonusDamage;
    public int bonusHealth;

    public override void Apply(Player player)
    {
        //this is an example idk what we want yet
        player.attackDamage += bonusDamage;
        player.maxHP += bonusHealth;
        player.AddBuff(buffID);
    }
}
