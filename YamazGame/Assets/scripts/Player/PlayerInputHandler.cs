using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 Move { get; private set; }

    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }
    public bool QPressed { get; private set; }
    public bool EPressed { get; private set; }

    public void OnMove(InputAction.CallbackContext ctx)
    {
        Move = ctx.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) DashPressed = true;
    }

    public void OnAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) AttackPressed = true;
    }

    public void OnAbilityQ(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) QPressed = true;
    }

    public void OnAbilityE(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) EPressed = true;
    }

    private void LateUpdate()
    {
        // Reset one-frame presses AFTER everyone has read them in Update()
        DashPressed = false;
        AttackPressed = false;
        QPressed = false;
        EPressed = false;
    }
}


