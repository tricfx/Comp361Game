using UnityEngine;

public class BossAnimatorController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private static readonly int MoveX = Animator.StringToHash("MoveX");
    private static readonly int MoveY = Animator.StringToHash("MoveY");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int FaceX = Animator.StringToHash("FaceX");
    private static readonly int FaceY = Animator.StringToHash("FaceY");
    private static readonly int Death = Animator.StringToHash("Death");
    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int AttackID = Animator.StringToHash("AttackID"); // Which attack (1,2,3...)
    private static readonly int Charge = Animator.StringToHash("Charge");

    private void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
    }

    public void SetMove(Vector2 dir)
    {
        animator.SetFloat(MoveX, dir.x);
        animator.SetFloat(MoveY, dir.y);
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat(Speed, speed);
    }

    public void SetFacing(Vector2 dir)
    {
        animator.SetFloat(FaceX, dir.x);
        animator.SetFloat(FaceY, dir.y);
    }

    public void TriggerAttack(int attackID)
    {
        animator.SetInteger(AttackID, attackID);
        animator.SetTrigger(Attack);
    }

    public void TriggerCharge()
    {
        animator.SetTrigger(Charge);
    }

    public void TriggerDeath()
    {
        animator.SetTrigger(Death);
    }
}