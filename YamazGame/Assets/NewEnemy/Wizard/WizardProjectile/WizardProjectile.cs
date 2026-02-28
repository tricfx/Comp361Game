using Unity.VisualScripting;
using UnityEngine;

public class WizardProjectile : MonoBehaviour
{
    [SerializeField] int projectileSpeed = 5000;
    [SerializeField] float projectileDuration = 2f;

    Animator animator;
    Rigidbody2D rb;
    Collider2D hurtbox;

    Collider2D targetHitbox;
    float timer;
    bool exploded = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        hurtbox = GetComponent<Collider2D>();
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
        if (targetHitbox == null || exploded)
        {
            return;
        }

        Vector2 playerPosition = targetHitbox.bounds.center;
        Vector2 direction = (playerPosition - (Vector2) transform.position).normalized;
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
        if (!other.CompareTag("PlayerHitbox")) return;
        Explode();
    }
}
