using UnityEngine;

public class DeathbringerSpellSpawner : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] EnemyDetectionRange detectionRange;

    public void SpawnSpell()
    {
        if (!detectionRange.PlayerInRange) return;
        Instantiate(spellPrefab, detectionRange.PlayerPosition, Quaternion.identity);
    }
}
