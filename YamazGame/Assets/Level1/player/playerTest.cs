using UnityEngine;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;



    private Rigidbody2D rb;
    private Vector2 movement;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;
    }

    void FixedUpdate()
    {
       
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = movement * moveSpeed;
    }
}
