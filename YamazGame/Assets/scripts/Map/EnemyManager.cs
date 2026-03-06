using UnityEngine;
using System;
using System.Diagnostics;

public class EnemyManager : MonoBehaviour
{
    public static event Action OnAllEnemiesDefeated;

    private int aliveEnemies = 0;

    private void Start()
    {
        UnityEngine.Debug.Log($"Total enemies {aliveEnemies}");
        if (aliveEnemies == 0)
        {
            UnityEngine.Debug.LogWarning("No enemies registered in scene.");
            OnAllEnemiesDefeated.Invoke();
        }
    }

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
}