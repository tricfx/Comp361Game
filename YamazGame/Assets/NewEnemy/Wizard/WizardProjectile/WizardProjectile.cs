using Unity.VisualScripting;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] int projectileSpeed = 5000;
    [SerializeField] float projectileDuration = 2f;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip clip;
    [SerializeField] protected float volume = 1f;

    Animator animator;
    Rigidbody2D rb;
    Collider2D hurtbox;

    NewEnemy ownerEnemy;

    public void SetOwner(NewEnemy owner)
    {
        ownerEnemy = owner;
    }

    Collider2D targetHitbox;
    float timer;
    bool exploded = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= projectileDuration)
        {
            Explode();
            return;
        }
    }

    void FixedUpdate()
    {
        if (exploded)
        {
            return;
        }

        Vector2 targetPosition;

        if (ownerEnemy != null && ownerEnemy.CurrentTarget != null)
        {
            // Wizard has a target (enemy or player)
            targetPosition = ownerEnemy.CurrentTarget.position;
        }
        else
        {
            // Fallback to the original target (usually the player)
            if (targetHitbox == null) return;
            targetPosition = targetHitbox.bounds.center;
        }

        Vector2 direction = (targetPosition - (Vector2) transform.position).normalized;
        rb.AddForce(direction * projectileSpeed * Time.fixedDeltaTime);

        if (rb.linearVelocity.sqrMagnitude > 0.001f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    public void SetTarget(Collider2D newTarget)
    {
        targetHitbox = newTarget;
    }

    void Explode()
    {   
        if (exploded) return;
        exploded = true;

        audioSource.PlayOneShot(clip, volume);

        rb.linearVelocity = Vector2.zero;
        animator.SetTrigger("explode");
        hurtbox.enabled = false;
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore collisions with the wizard that fired the projectile
        NewEnemy hitEnemy = other.GetComponentInParent<NewEnemy>();
        if (ownerEnemy != null && hitEnemy == ownerEnemy)
        {
            return;
        }

        // If wizard is charmed, projectile should damage enemies
        if (ownerEnemy != null && ownerEnemy.CurrentTeam == NewEnemy.Team.Ally)
        {
            if (ownerEnemy.CurrentTarget == null) return;

            NewEnemy targetEnemy = ownerEnemy.CurrentTarget.GetComponent<NewEnemy>();
            if (targetEnemy == null || !targetEnemy.IsAlive) return;

            // Only explode when hitting the enemy's main body collider (feetCollider)
            if (other != targetEnemy.feetCollider) return;

            targetEnemy.TakeDamage(2);
            Explode();
            return;
        }

        // Normal behaviour → damage player
        if (!other.CompareTag("PlayerHitbox")) return;

        Explode();
    }
}
