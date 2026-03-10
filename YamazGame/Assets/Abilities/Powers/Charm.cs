using UnityEngine;
using System.Collections;

public class Charm : MonoBehaviour, IAbility
{
    [SerializeField] private float duration = 15f;
    [SerializeField] private float charmDuration = 15f;
    [SerializeField] private float detectionWindow = 2f;
    [SerializeField] private float charmRange = 2.0f;
    [SerializeField] private PlayerHealth playerHealth;
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
    }

    public void Do()
    {
        // Stop any previous charm coroutine so timers reset correctly
        StopAllCoroutines();

        if (charmInstance != null)
        {
            charmInstance.Play();
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
                detecting = false;

            if (charmInstance != null)
                charmInstance.transform.position = playerHealth.transform.position;

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
