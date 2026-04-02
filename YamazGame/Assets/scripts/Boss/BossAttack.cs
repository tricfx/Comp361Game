using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] public int attackDamage = 20;
    [SerializeField] public float attackCooldown = 2f;

    [SerializeField] private BossAnimatorController anim;
    [SerializeField] private BossMovement movement;

    private Transform player;
    private float lastAttackTime = -Mathf.Infinity;
    private bool stageComplete = false;
    private float stageStartTime = 0f;

    [Header("Attack Timing")]
    [SerializeField] private float stageTimeout = 2.5f; // fallback if animation event is missing

    public bool IsAttacking { get; private set; } = false;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown && !IsAttacking;

    private void Awake()
    {
        if (!anim) anim = GetComponent<BossAnimatorController>();
        if (!movement) movement = GetComponent<BossMovement>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    // Called by BossAI to trigger close range attack
    public void PerformCloseRangeAttack()
    {
        if (!CanAttack) return;

        StopAllCoroutines();
        IsAttacking = true;
        lastAttackTime = Time.time;
        movement?.Stop();
        StartCoroutine(CloseRangeRoutine());
    }

    // Called when AI needs to interrupt the attack (e.g. player left range)
    public void CancelAttack()
    {
        StopAllCoroutines();
        anim.SetAttackStage(0);
        IsAttacking = false;
    }

    private IEnumerator CloseRangeRoutine()
    {
        IsAttacking = true;

        // Stage 1 — Sword Left
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(1);
        anim.TriggerAttack(1); // ← THIS was missing — pokes the animator to transition
        stageComplete = false;
        stageStartTime = Time.time;
        yield return new WaitUntil(() => stageComplete || WaitTimeout(stageTimeout));

        // Stage 2 — Sword Right
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(2);
        anim.TriggerAttack(2);
        stageComplete = false;
        stageStartTime = Time.time;
        yield return new WaitUntil(() => stageComplete || WaitTimeout(stageTimeout));

        // Stage 3 — Laser
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(3);
        anim.TriggerAttack(3);
        stageComplete = false;
        stageStartTime = Time.time;
        yield return new WaitUntil(() => stageComplete || WaitTimeout(stageTimeout));

        // Reset and wait one frame before checking idle
        // (gives animator time to actually start transitioning back)
        anim.SetAttackStage(0);
        IsAttacking = false;
    }

    private Animator _animator;
    private bool IsInIdleState()
    {
        if (_animator == null) _animator = GetComponentInChildren<Animator>();
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        // Must be in idle AND not in any transition
        return info.IsName("Idle") && !_animator.IsInTransition(0);
    }

    // Snaps direction to nearest cardinal (N/S/E/W)
    private Vector2 GetCardinalToPlayer()
    {
        if (player == null) return Vector2.down;

        Vector2 dir = (player.position - transform.position).normalized;

        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return dir.x >= 0 ? Vector2.right : Vector2.left;   // E or W
        else
            return dir.y >= 0 ? Vector2.up : Vector2.down;      // N or S
    }

    // Called by animation event on the last frame of each stage clip
    public void OnStageComplete()
    {
        stageComplete = true;
    }

    private bool WaitTimeout(float timeout)
    {
        return Time.time - stageStartTime >= timeout;
    }
}