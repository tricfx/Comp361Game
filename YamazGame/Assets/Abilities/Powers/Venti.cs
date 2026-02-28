using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;

public class Venti : MonoBehaviour, IAbility
{
    [SerializeField] private Collider2D mapBounds;
    [SerializeField] private float spawnOffset = 10f;
    [SerializeField] private float boundMargin = 3f; // spawn 2f before the wall/bound
    [SerializeField] private float duration = 10f;
    [SerializeField] private PlayerHealth playerHealth;
    public ParticleSystem venti;
    private ParticleSystem ventiInstance;



    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();

        Vector3 spawnPosition = playerHealth.transform.position;

        ventiInstance = Instantiate(venti, GetSpawnPosition(), Quaternion.identity);
    }

    public void Do()
    {

        if (ventiInstance != null)
        {
            ventiInstance.Play();
            if (ventiInstance != null)
                ventiInstance.transform.position = GetSpawnPosition();
        }
        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (ventiInstance != null)
            ventiInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        float total = 0f;

        while (total < duration)
        {
            

            total += Time.deltaTime;
            yield return null; // wait one frame
        }

        if (ventiInstance != null)
            ventiInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
    private Vector3 GetSpawnPosition()
    {
        var controller = playerHealth.GetComponentInParent<PlayerController2D>();
        Vector2 dir2D = controller != null ? controller.MoveDir : Vector2.right;

        Vector3 dir = new Vector3(dir2D.x, dir2D.y, 0f).normalized;
        Vector3 origin = playerHealth.transform.position;

        float maxCheck = spawnOffset + boundMargin;

        // Raycast and look for the closest "map" collider automatically
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, dir, maxCheck);

        float nearest = float.PositiveInfinity;
        foreach (var hit in hits)
        {
            if (!hit.collider) continue;

            // ignore player’s own colliders
            if (hit.collider.transform.IsChildOf(playerHealth.transform.root)) continue;

            // treat these as map bounds (tilemap walls/floor composites)
            if (hit.collider is TilemapCollider2D || hit.collider is CompositeCollider2D)
            {
                if (hit.distance < nearest) nearest = hit.distance;
            }
        }

        float travel = spawnOffset;

        // If we hit a bound, stop 2 units before it
        if (nearest < float.PositiveInfinity)
            travel = Mathf.Max(0f, nearest - boundMargin);

        return origin + dir * travel;
    }
}

