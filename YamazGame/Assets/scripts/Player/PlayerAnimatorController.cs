using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int AttackStep = Animator.StringToHash("AttackStep");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int FaceX = Animator.StringToHash("FaceX");
    private static readonly int FaceY = Animator.StringToHash("FaceY");
    private static readonly int IsDashing = Animator.StringToHash("IsDashing");
    private static readonly int Dash = Animator.StringToHash("Dash");
    private static readonly int Attack = Animator.StringToHash("Attack");


    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    public void SetMove(Vector2 moveDir)
    {
        animator.SetFloat(MoveX, moveDir.x);
        animator.SetFloat(MoveY, moveDir.y);
    }

    public void SetSpeed(float speed01)
    {
        animator.SetFloat(Speed, speed01);
    }

    public void SetFacing(Vector2 faceDir)
    {
        animator.SetFloat(FaceX, faceDir.x);
        animator.SetFloat(FaceY, faceDir.y);
    }

    public void SetDashing(bool dashing)
    {
        animator.SetBool(IsDashing, dashing);
    }

    public void TriggerDash()
    {

        animator.SetTrigger(Dash);
    }

    public void TriggerAttack()
    {
        animator.SetTrigger(Attack);
    }
    public void TriggerAttackCombo(int step)
    {
        if (!animator) return;
        animator.SetInteger(AttackStep, step);
        animator.SetTrigger(Attack);

    }
    public void TriggerDeath()
    {
        if (!animator) return;
        animator.SetTrigger(Death);

    }

    public void TriggerSepuku()
    {
        if (!animator) return;
        animator.SetTrigger("Seppuku");
    }

    public void ResetAttackStep()
    {
        if (!animator) return;
        animator.SetInteger(AttackStep, 0);

    }

    public void EnableCombo()
    {
        GetComponent<PlayerActions>()?.AllowNextCombo();
    }


}
