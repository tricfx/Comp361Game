using Unity.VisualScripting;
using UnityEngine;

public class WizardProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;

    public void SpawnProjectile(Transform playerRoot)
    {
        Collider2D playerHitbox = null;

        foreach (Transform child in playerRoot)
        {
            if (child.CompareTag("PlayerHitbox"))
            {
                playerHitbox = child.GetComponent<Collider2D>();
                break;
            }
        }

        if (playerHitbox == null)
        {
            Debug.LogWarning("Player hitbox not set");
            return;
        }

        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        WizardProjectile projectile = proj.GetComponent<WizardProjectile>();
        projectile.SetTarget(playerHitbox);
    }
}
