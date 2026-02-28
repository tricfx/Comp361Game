using UnityEngine;

public class EnemyAttackRange : MonoBehaviour
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

    bool _playerInRange;
    Transform _playerTransform;

    void Start()
    {
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
        if (other.gameObject.CompareTag("PlayerHitbox"))
        {
            _playerInRange = false;
            _playerTransform = null;
        }
    }
}
