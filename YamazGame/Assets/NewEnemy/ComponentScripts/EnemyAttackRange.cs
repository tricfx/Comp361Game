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
            return _playerPosition;
        }
    }

    bool _playerInRange;
    Vector2 _playerPosition;

    void Start()
    {
        _playerPosition = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerInRange = true;
            _playerPosition = other.gameObject.transform.position;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playerInRange = false;
            _playerPosition = Vector2.zero;
        }
    }
}
