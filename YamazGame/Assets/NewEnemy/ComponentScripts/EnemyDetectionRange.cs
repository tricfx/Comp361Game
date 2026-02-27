using UnityEngine;

public class EnemyDetectionRange : MonoBehaviour
{
    public bool PlayerInRange
    {
        get
        {
            return _playerInRange;
        }
    }

    public Vector2 PlayerPosition
    {
        get
        {
            return _playerTransform.position;
        }
    }

    [SerializeField] float radius = 3;
    bool _playerInRange;
    Transform _playerTransform;

    void Start()
    {
        gameObject.GetComponent<CircleCollider2D>().radius = radius;
        _playerTransform = null;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            _playerInRange = true;
            _playerTransform = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("PlayerHitbox"))
        {
            _playerInRange = false;
            _playerTransform = null;
        }
    }
}
