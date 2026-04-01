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

        IsAttacking = true;
        lastAttackTime = Time.time;
        movement?.Stop();
        StartCoroutine(CloseRangeRoutine());
    }

    private IEnumerator CloseRangeRoutine()
    {
        // Stage 1 — Sword Left
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(1);
        stageComplete = false;
        yield return new WaitUntil(() => stageComplete);

        // Stage 2 — Sword Right
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(2);
        stageComplete = false;
        yield return new WaitUntil(() => stageComplete);

        // Stage 3 — Laser
        anim.SetFacing(GetCardinalToPlayer());
        anim.SetAttackStage(3);
        stageComplete = false;
        yield return new WaitUntil(() => stageComplete);

        // Done — return to idle
        anim.SetAttackStage(0);
        IsAttacking = false;
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
}
