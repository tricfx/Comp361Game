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
        {
            manager.NotifySpawningStarted();
        }
        
        int availableTypes = Mathf.Min(Mathf.Max(2, levelNumber), allEnemies.Count);

        List<Transform> shuffledSpawnPoints = new List<Transform>(spawnPoints);
        for (int i = 0; i < shuffledSpawnPoints.Count; i++)
        {
            Transform temp = shuffledSpawnPoints[i];
            int randomIndex = Random.Range(i, shuffledSpawnPoints.Count);
            shuffledSpawnPoints[i] = shuffledSpawnPoints[randomIndex];
            shuffledSpawnPoints[randomIndex] = temp;
        }

        List<Vector3> usedPositions = new List<Vector3>();

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            int randomIndex = Random.Range(0, availableTypes);
            GameObject prefab = allEnemies[randomIndex].prefab;

            Transform spawnPoint = shuffledSpawnPoints[i % shuffledSpawnPoints.Count];

            Vector3 spawnPosition = spawnPoint.position;
            bool validPosition = false;
            int attempts = 0;

            while (!validPosition && attempts < 10)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-2.5f, 2.5f),
                    Random.Range(-2.5f, 2.5f),
                    0
                );

                Vector3 candidatePosition = spawnPoint.position + offset;

                // Prevent enemies spawning too close to each other
                bool tooClose = false;
                foreach (var pos in usedPositions)
                {
                    if (Vector3.Distance(pos, candidatePosition) < 1.5f)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (tooClose)
                {
                    attempts++;
                    continue;
                }

                // Check ANY collider at position (ignore triggers)
                Collider2D hit = Physics2D.OverlapCircle(candidatePosition, 0.4f);

                if (hit == null)
                {
                    spawnPosition = candidatePosition;
                    validPosition = true;
                }

                attempts++;
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