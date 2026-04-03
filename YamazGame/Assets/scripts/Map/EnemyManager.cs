using UnityEngine;
using System.Collections.Generic;
using System;
using System.Diagnostics;

public class EnemyManager : MonoBehaviour
{
    public static event Action OnAllEnemiesDefeated;
    private bool hasStartedSpawning = false;

    private int aliveEnemies = 0;


    public void RegisterEnemy()
    {
        aliveEnemies++;
    }

    public void UnregisterEnemy()
    {
        aliveEnemies--;

        if (aliveEnemies <= 0)
        {
            UnityEngine.Debug.Log("All enemies defeated");
            OnAllEnemiesDefeated?.Invoke();
        }
    }

    public void NotifySpawningStarted()
    {
        hasStartedSpawning = true;
    }
}
