using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityCard : Card
{
    public string abilityID;
    public GameObject abilityPrefab;

    public override void Apply(Player player)
    {
        player.TryEquipAbility(this);
    }
}