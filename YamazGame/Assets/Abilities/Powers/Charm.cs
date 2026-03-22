using UnityEngine;
using System.Collections;

public class Charm : MonoBehaviour, IAbility
{
    [SerializeField] private float duration = 15f;
    [SerializeField] private float charmDuration = 15f;
    [SerializeField] private float detectionWindow = 4f;
    [SerializeField] private float charmRange = 2.0f;
    [SerializeField] private float auraScaleMultiplier = 2f; // adjusts visual size of the circle sprite
    [SerializeField] private float auraGrowSpeed = 3f; // higher value = circle expands faster
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject rangeAura;
    private GameObject rangeAuraInstance;
    public ParticleSystem charm;
    private ParticleSystem charmInstance;

    private float detectionTimer = 0f;
    private bool detecting = false;

    private void Awake()
    {    
        if (!playerHealth)
            playerHealth = FindFirstObjectByType<PlayerHealth>();

        Vector3 spawnPosition = playerHealth.transform.position;

        charmInstance = Instantiate(charm, spawnPosition, Quaternion.identity);

        // Create the circular range aura (initially disabled)
        if (rangeAura != null)
        {
            rangeAuraInstance = Instantiate(rangeAura, spawnPosition, Quaternion.identity);

            // start hidden and with scale 0 so it can grow when the ability starts
            rangeAuraInstance.transform.localScale = Vector3.zero;
            rangeAuraInstance.SetActive(false);
        }
    }

    public void Do()
    {
        // Stop any previous charm coroutine so timers reset correctly
        StopAllCoroutines();

        if (charmInstance != null)
        {
            charmInstance.Play();
            if (rangeAuraInstance != null)
                rangeAuraInstance.SetActive(true);
            detectionTimer = 0f;
            detecting = true;
        }

        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (charmInstance != null)
            charmInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        float total = 0f;

        while (total < duration)
        {
            detectionTimer += Time.deltaTime;

            if (detectionTimer > detectionWindow)
            {
                detecting = false;

                // hide the aura exactly when the detection window ends
                if (rangeAuraInstance != null)
                    rangeAuraInstance.SetActive(false);
            }

            if (charmInstance != null)
                charmInstance.transform.position = playerHealth.transform.position;

            if (rangeAuraInstance != null)
            {
                rangeAuraInstance.transform.position = playerHealth.transform.position;

                if (detecting)
                {
                    float normalizedTime = Mathf.Clamp01(detectionTimer / detectionWindow);

                    // Ease‑out growth (fast start, slow end)
                    float growthT = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(normalizedTime * auraGrowSpeed));
                    float diameter = charmRange * 2f * auraScaleMultiplier;

                    rangeAuraInstance.transform.localScale =
                        Vector3.Lerp(Vector3.zero, new Vector3(diameter, diameter, 1f), growthT);

                    // Fade only during the last second
                    SpriteRenderer sr = rangeAuraInstance.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        float fadeStart = Mathf.Max(0f, detectionWindow - 1f) / detectionWindow;

                        float alpha;
                        if (normalizedTime < fadeStart)
                            alpha = 0.35f;
                        else
                            alpha = Mathf.Lerp(0.35f, 0f, (normalizedTime - fadeStart) / (1f - fadeStart));

                        Color c = sr.color;
                        c.a = alpha;
                        sr.color = c;
                    }
                }
            }

            transform.position = playerHealth.transform.position;
            
            total += Time.deltaTime;
            yield return null; // wait one frame
        }

        if (charmInstance != null)
            charmInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!detecting) return;

        if (!other.CompareTag("Enemy")) return; // Do not collide with things that are not enemies 

        NewEnemy enemy = other.GetComponentInParent<NewEnemy>();
        if (enemy != null)
        {
            float dist = Vector2.Distance(playerHealth.transform.position, enemy.transform.position);
            if (dist <= charmRange)
            {
                enemy.ApplyCharm(charmDuration);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!detecting) return;

        NewEnemy enemy = other.GetComponentInParent<NewEnemy>();

        if (enemy != null)
        {
            float dist = Vector2.Distance(playerHealth.transform.position, enemy.transform.position);
            if (dist <= charmRange)
            {
                enemy.ApplyCharm(charmDuration);
            }
        }
    }
}
