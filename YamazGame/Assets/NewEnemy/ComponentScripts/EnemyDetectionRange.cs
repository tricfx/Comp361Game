using UnityEngine;

public class EnemyDetectionRange : MonoBehaviour
{
    public bool PlayerInRange => _player != null;

    public Vector2 PlayerPosition
    {
        get
        {
            if (_player == null) return Vector2.zero;
            return _player.feetCollider.bounds.center;
        }
    }

    public PlayerController2D Player => _player;

    [SerializeField] float radius = 3f;
    PlayerController2D _player;

    void Awake()
    {
        GetComponent<CircleCollider2D>().radius = radius;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            _player = other.GetComponentInParent<PlayerController2D>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            _player = null;
        }
    }
}
