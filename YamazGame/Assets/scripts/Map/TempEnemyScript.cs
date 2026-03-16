using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyManager enemyManager;
    private bool isDead = false;

    private void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();
        enemyManager.RegisterEnemy();
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        enemyManager.UnregisterEnemy();
        Destroy(gameObject);
    }
}