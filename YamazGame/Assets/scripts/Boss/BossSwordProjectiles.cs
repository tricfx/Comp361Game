using UnityEngine;
public class BossSwordProjectile : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifetime = 5f; // auto-destroy if it misses

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            PlayerHealth player = other.GetComponentInParent<PlayerHealth>();
            player?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
