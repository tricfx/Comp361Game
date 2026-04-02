using UnityEngine;
using System.Collections;

public class BossAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] public float attackCooldown = 2.25f;

    [SerializeField] private BossAnimatorController anim;
    [SerializeField] private BossMovement movement;

    private Transform player;
    private float lastAttackTime = -Mathf.Infinity;
    private bool stageComplete = false;
    private float stageStartTime = 0f;

    [Header("Attack Timing")]
    [SerializeField] private float stageTimeout = 10f;

    public bool IsAttacking { get; private set; } = false;
    public bool CanAttack => Time.time >= lastAttackTime + attackCooldown && !IsAttacking;

    private void Awake()
    {
        if (!anim) anim = GetComponent<BossAnimatorController>();
        if (!movement) movement = GetComponent<BossMovement>();

        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    public void PerformCloseRangeAttack()
    {
        if (!CanAttack) return;

        StopAllCoroutines();

        // Set IsAttacking = true BEFORE starting the coroutine so the AI
        // sees it immediately on the next Update frame and doesn't re-enter attack
        IsAttacking = true;
        lastAttackTime = Time.time;
        movement?.Stop();
        StartCoroutine(CloseRangeRoutine());
    }

    public void CancelAttack()
    {
        StopAllCoroutines();
        anim.SetAttackStage(0);
        IsAttacking = false;
        // Reset cooldown so the boss can attack again promptly after re-engaging
        lastAttackTime = -Mathf.Infinity;
    }

    private IEnumerator CloseRangeRoutine()
    {
        // Face player once at the start of the combo only
        anim.SetFacing(GetCardinalToPlayer());

        for (int stage = 1; stage <= 3; stage++)
        {
            anim.SetAttackStage(stage);
            stageComplete = false;
            stageStartTime = Time.time;
            yield return new WaitUntil(() => stageComplete || WaitTimeout(stageTimeout));
        }
        OnStageComplete(); // Ensure we reset stageComplete in case of timeout

        anim.SetAttackStage(0);
        IsAttacking = false;
    }

    private Vector2 GetCardinalToPlayer()
    {
        if (player == null) return Vector2.down;

        Vector2 dir = (player.position - transform.position).normalized;

        if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
            return dir.x >= 0 ? Vector2.right : Vector2.left;
        else
            return dir.y >= 0 ? Vector2.up : Vector2.down;
    }

    // Called by animation event on last frame of each stage clip
    public void OnStageComplete()
    {
        stageComplete = true;
    }

    private bool WaitTimeout(float timeout)
    {
        return Time.time - stageStartTime >= timeout;
    }
}