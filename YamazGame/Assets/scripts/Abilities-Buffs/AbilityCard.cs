using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Ability")]
public class AbilityCard : Card
{
    public string abilityID;
    public GameObject abilityPrefab;

    public override void Apply(GameObject playerObject)
    {
        var actions = playerObject.GetComponent<PlayerActions>();
        if (actions != null)
            actions.TryEquipAbility(this);
        else
            Debug.LogWarning("AbilityCard: No PlayerActions found on player object.");
    }
}