using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Buff")]
public class BuffCard : Card
{
    public string buffID;
    public int bonusDamage;
    public int bonusHealth;
    public float bonusSpeed;
    public float dashCooldownDecrease;

    public override void Apply(GameObject playerObject)
    {
        // Attack damage lives on PlayerHitbox
        var hitbox = playerObject.GetComponentInChildren<PlayerHitbox>();
        if (hitbox != null)
            hitbox.attackDamage += bonusDamage;

        // Max health lives on PlayerHealth
        var health = playerObject.GetComponent<PlayerHealth>();
        if (health != null)
            health.maxHealth += bonusHealth;

        // Move speed lives on PlayerController2D
        var controller = playerObject.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.moveSpeed += bonusSpeed;

        // Dash cooldown lives on PlayerActions
        var actions = playerObject.GetComponent<PlayerActions>();
        if (actions != null)
            actions.dashCooldown = Mathf.Max(0f, actions.dashCooldown - dashCooldownDecrease);

        // Buffs tracked on PlayerBuffs
        var buffs = playerObject.GetComponent<PlayerBuffs>();
        if (buffs != null)
            buffs.AddBuff(buffID);
    }
}
