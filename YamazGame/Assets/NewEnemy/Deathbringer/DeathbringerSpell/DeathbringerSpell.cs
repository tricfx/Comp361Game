using UnityEngine;

public class DeathbringerSpell : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip clip;
    [SerializeField] protected float volume = 1f;

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

    void PlaySound()
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
