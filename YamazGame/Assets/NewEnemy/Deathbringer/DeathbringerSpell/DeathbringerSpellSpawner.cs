using UnityEngine;

public class DeathbringerSpellSpawner : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] EnemyDetectionRange detectionRange;
    [SerializeField] Vector2 shadowOffset;

    public GameObject SpawnSpell()
    {
        if (!detectionRange.PlayerInRange) return null;

        Vector3 spawnPos = detectionRange.PlayerPosition - shadowOffset;

        return Instantiate(spellPrefab, spawnPos, Quaternion.identity);
    }
}
