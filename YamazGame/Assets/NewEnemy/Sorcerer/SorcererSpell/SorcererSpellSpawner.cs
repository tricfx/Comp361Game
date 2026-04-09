using UnityEngine;

public class SorcererSpellSpawner : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] EnemyDetectionRange detectionRange;

    public GameObject SpawnSpell()
    {
        if (!detectionRange.PlayerInRange) return null;
        return Instantiate(spellPrefab, detectionRange.PlayerPosition, Quaternion.identity);
    }

    public GameObject SpawnSpellAt(Vector2 targetPosition)
    {
        return Instantiate(spellPrefab, targetPosition, Quaternion.identity);
    }
}
