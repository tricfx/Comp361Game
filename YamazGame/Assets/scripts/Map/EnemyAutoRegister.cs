using UnityEngine;
using System.Collections.Generic;

public class EnemyAutoRegister : MonoBehaviour
{
    private EnemyManager enemyManager;

    private void Start()
    {
        enemyManager = FindFirstObjectByType<EnemyManager>();

        if (enemyManager != null)
        {
            enemyManager.RegisterEnemy();
        }
    }

    private void OnDestroy()
    {
        if (enemyManager != null)
        {
            enemyManager.UnregisterEnemy();
        }
    }
}
