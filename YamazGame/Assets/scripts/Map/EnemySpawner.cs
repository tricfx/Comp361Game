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

    [Header("Spawn points")]
    [SerializeField] private Transform[] spawnPoints;

    public void SpawnEnemies()
    {
        EnemyManager manager = FindFirstObjectByType<EnemyManager>();
        if (manager != null)
            manager.NotifySpawningStarted();

        int availableTypes = Mathf.Min(Mathf.Max(2, levelNumber), allEnemies.Count);

        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            int r = Random.Range(i, shuffledSpawnPoints.Count);
            Transform tmp = shuffledSpawnPoints[i];
            shuffledSpawnPoints[i] = shuffledSpawnPoints[r];
            shuffledSpawnPoints[r] = tmp;
        }

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            GameObject prefab = allEnemies[Random.Range(0, availableTypes)].prefab;
            Transform spawnPoint = shuffledSpawnPoints[i % shuffledSpawnPoints.Count];

            Vector3 spawnPosition = spawnPoint.position;

            for (int attempt = 0; attempt < 15; attempt++)
            {
                Vector3 candidate = spawnPoint.position + new Vector3(
                    Random.Range(-8f, 8f),
                    Random.Range(-8f, 8f),
                    0
                );

                bool tooClose = false;
                foreach (Vector3 used in usedPositions)
                {
                    if (Vector3.Distance(candidate, used) < 2f)
                    {
                        tooClose = true;
                        break;
                    }
                }

                int blockingMask = LayerMask.GetMask("Walls", "Lava");
                bool nearCollider = Physics2D.OverlapCircle(candidate, 0.5f, blockingMask) != null;

                if (!tooClose && !nearCollider)
                {
                    spawnPosition = candidate;
                    break;
                }
            }

            usedPositions.Add(spawnPosition);
            Instantiate(prefab, spawnPosition, Quaternion.identity);
        }
    }

    private void Start()
    {
        SpawnEnemies();
    }
}