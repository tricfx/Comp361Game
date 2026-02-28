using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Buff")]
public class BuffCard : Card
{
    public string buffID;
    public int bonusDamage;
    public int bonusHealth;
    public float bonusSpeed;
    public float dashCooldownDecrease;
    public float dashDistanceBonus;

    public override void Apply(GameObject playerObject)
    {
        // Attack damage lives on PlayerHitbox
        var hurtbox = playerObject.GetComponentInChildren<PlayerHurtbox>();
        if (hurtbox != null)
            hurtbox.attackDamage += bonusDamage;

        // Max health lives on PlayerHealth
        var health = playerObject.GetComponent<PlayerHealth>();
        if (health != null)
            health.currentHealth += bonusHealth;
            health.maxHealth += bonusHealth;

        // Move speed lives on PlayerController2D
        var controller = playerObject.GetComponent<PlayerController2D>();
        if (controller != null)
            controller.moveSpeed += bonusSpeed;

        // Dash cooldown lives on PlayerActions
        var movementDash = playerObject.GetComponent<PlayerController2D>();
        if (movementDash != null)
            movementDash.dashCooldown = Mathf.Max(0f, movementDash.dashCooldown - dashCooldownDecrease);

        // Buffs tracked on PlayerBuffs
        var buffs = playerObject.GetComponent<PlayerBuffs>();
        if (buffs != null)
            buffs.AddBuff(buffID);

        var distance = playerObject.GetComponent<PlayerController2D>();
        if (distance != null)
            distance.dashDistance += dashDistanceBonus;

    }
}
