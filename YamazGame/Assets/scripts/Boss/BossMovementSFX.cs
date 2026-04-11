using UnityEngine;

public class BossMovementSfx : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private AudioSource walkLoopSfx;
    [SerializeField] private float moveThreshold = 0.05f;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool isMoving = rb != null && rb.linearVelocity.magnitude > moveThreshold;

        if (isMoving)
        {
            if (walkLoopSfx != null && !walkLoopSfx.isPlaying)
                walkLoopSfx.Play();
        }
        else
        {
            if (walkLoopSfx != null && walkLoopSfx.isPlaying)
                walkLoopSfx.Stop();
        }
    }

    private void OnDisable()
    {
        if (walkLoopSfx != null)
            walkLoopSfx.Stop();
    }

    private void OnDestroy()
    {
        if (walkLoopSfx != null)
            walkLoopSfx.Stop();
    }
}