using UnityEngine;

public class WizardProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;

    public void SpawnProjectile()
    {
        GameObject hitboxObj = GameObject.FindGameObjectWithTag("PlayerHitbox");

        if (hitboxObj == null)
        {
            Debug.LogWarning("hitbox object not found");
        }

        Collider2D targetHitbox = hitboxObj.GetComponent<Collider2D>();

        if (targetHitbox == null)
        {
            Debug.LogWarning("Player hitbox not set");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        WizardProjectile projectile = proj.GetComponent<WizardProjectile>();

        // set player target (default behavior)
        projectile.SetTarget(targetHitbox);

        // pass the wizard that fired the projectile
        NewEnemy owner = GetComponentInParent<NewEnemy>();
        if (owner != null)
        {
            projectile.SetOwner(owner);
        }
    }
}
