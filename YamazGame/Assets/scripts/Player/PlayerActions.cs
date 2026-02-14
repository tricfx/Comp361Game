using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInputHandler input;
    [SerializeField] private PlayerAnimatorController anim;
    [SerializeField] private PlayerController2D controller; // optional (block during dash)

    private void Awake()
    {
        if (!input) input = GetComponent<PlayerInputHandler>();
        if (!anim) anim = GetComponent<PlayerAnimatorController>();
        if (!controller) controller = GetComponent<PlayerController2D>();
    }

    private void Update()
    {
        // Optional: don't allow actions while dashing
        if (controller && controller.IsDashing)
            return;

        if (input.AttackPressed)
        {
            anim?.TriggerAttack();
            Debug.Log("Attack (placeholder)");
        }

        if (input.QPressed)
        {
            Debug.Log("Ability Q (placeholder)");
        }

        if (input.EPressed)
        {
            Debug.Log("Ability E (placeholder)");
        }
    }
}

