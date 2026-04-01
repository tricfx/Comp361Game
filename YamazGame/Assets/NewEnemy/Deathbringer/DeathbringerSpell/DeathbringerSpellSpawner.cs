using UnityEngine;

public class DeathbringerSpellSpawner : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] EnemyDetectionRange detectionRange;

    public GameObject SpawnSpell()
    {
        if (!detectionRange.PlayerInRange) return null;
        return Instantiate(spellPrefab, detectionRange.PlayerPosition, Quaternion.identity);
    }
}
