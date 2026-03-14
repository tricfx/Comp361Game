using UnityEngine;

public class SorcererSpellSpawner : MonoBehaviour
{
    [SerializeField] GameObject spellPrefab;
    [SerializeField] EnemyDetectionRange detectionRange;

    public void SpawnSpell()
    {
        if (!detectionRange.PlayerInRange) return;
        Instantiate(spellPrefab, detectionRange.PlayerPosition, Quaternion.identity);
    }
}
