using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class EnemyEntry
{
    public GameObject prefab;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("All enemy types (ordered by unlock)")]
    [SerializeField] private List<EnemyEntry> allEnemies;

    [Header("Level settings")]
    [SerializeField] private int levelNumber;
    [SerializeField] private int totalEnemiesToSpawn = 10;

    [Header("Spawn areas")]
    [SerializeField] private PolygonCollider2D[] spawnAreas;

    public void SpawnEnemies()
    {
        if (spawnAreas == null || spawnAreas.Length == 0)
        {
            Debug.LogError("No spawn areas assigned!");
            return;
        }

        EnemyManager manager = FindFirstObjectByType<EnemyManager>();
        if (manager != null)
            manager.NotifySpawningStarted();

        int availableTypes = Mathf.Min(Mathf.Max(2, levelNumber), allEnemies.Count);
        int blockingMask = LayerMask.GetMask("Walls", "Lava");

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            GameObject prefab = allEnemies[Random.Range(0, availableTypes)].prefab;

            Vector3 spawnPosition = Vector3.positiveInfinity;

            for (int attempt = 0; attempt < 15; attempt++)
            {
                PolygonCollider2D area = spawnAreas[Random.Range(0, spawnAreas.Length)];
                Vector2 candidate = GetRandomPointInPolygon(area);

                bool nearBlocking = Physics2D.OverlapCircle(candidate, 0.6f, blockingMask) != null;
                if (nearBlocking) continue;

                bool tooClose = false;
                foreach (Vector3 used in usedPositions)
                {
                    if (Vector3.Distance(candidate, used) < 2f)
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose) continue;

                spawnPosition = candidate;
                break;
            }

            if (spawnPosition == Vector3.positiveInfinity) continue;

            usedPositions.Add(spawnPosition);

            GameObject enemyObject = Instantiate(prefab, spawnPosition, Quaternion.identity);

            NewEnemy enemy = enemyObject.GetComponent<NewEnemy>();
            if (enemy != null)
                enemy.InitializeForLevel(levelNumber);
        }
    }

    private Vector2 GetRandomPointInPolygon(PolygonCollider2D polygon)
    {
        Bounds bounds = polygon.bounds;

        for (int i = 0; i < 30; i++)
        {
            Vector2 candidate = new Vector2(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y)
            );

            if (polygon.OverlapPoint(candidate))
                return candidate;
        }

        return bounds.center;
    }

    private void Start()
    {
        SpawnEnemies();
    }
}
