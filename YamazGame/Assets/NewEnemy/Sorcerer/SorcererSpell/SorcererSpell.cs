using NUnit.Framework.Constraints;
using UnityEngine;

public class SorcererSpell : MonoBehaviour
{
    Collider2D hurtbox;
    bool hasHit = false;

    void Start()
    {
        hurtbox = GetComponent<Collider2D>();
    }

    void DestroySpell()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;
        if (!other.CompareTag("PlayerHitbox")) return;

        hasHit = true;
        hurtbox.enabled = false;
    }

    void EnableHurtbox()
    {
        if (hasHit) return;
        hurtbox.enabled = true;
    }

    void DisableHurtbox()
    {
        hurtbox.enabled = false;
    }
}
