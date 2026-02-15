using Unity.VisualScripting;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] float projectileDamage = 2f;
    [SerializeField] float projectileSpeed = 600f;
    [SerializeField] float projectileDuration = 3f;
    [SerializeField] float knockbackForce = 10f;

    SpriteRenderer spriteRenderer;
    Animator animator;
    Rigidbody2D rb;
    Collider2D hurtbox;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
    }

    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerHitbox")) return;

        PlayerController player = other.GetComponentInParent<PlayerController>();
        if (player == null || player.feetCollider == null) return;

    }
}
