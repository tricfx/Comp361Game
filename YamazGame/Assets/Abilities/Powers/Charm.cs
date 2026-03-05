using UnityEngine;
using System.Collections;

public class Charm : MonoBehaviour, IAbility
{
    [SerializeField] private float detectionDuration = 4f; // time the aura detects enemies
    [SerializeField] private float abilityDuration = 20f;  // total charm duration
    [SerializeField] private float charmRadius = 3f;
    [SerializeField] private PlayerHealth playerHealth;
    public ParticleSystem charm;
    private ParticleSystem charmInstance;
    private System.Collections.Generic.List<NewEnemy> charmedEnemies = new System.Collections.Generic.List<NewEnemy>();



    private void Awake()
    {    
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();

        Vector3 spawnPosition = playerHealth.transform.position;

        charmInstance = Instantiate(charm, spawnPosition, Quaternion.identity);
    }

    public void Do()
    {

        if (charmInstance != null)
        {
            charmInstance.Play();
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

        while (total < abilityDuration)
        {
            transform.position = playerHealth.transform.position;

            if (charmInstance != null)
                charmInstance.transform.position = playerHealth.transform.position;

            //Detect enemies only during the detection window
            if (total < detectionDuration)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(
                    playerHealth.transform.position,
                    charmRadius
                );

                foreach (Collider2D hit in hits)
                {
                    EnemyHurtbox hurtbox = hit.GetComponent<EnemyHurtbox>();
                    if (hurtbox == null) continue;

                    NewEnemy enemy = hurtbox.GetComponentInParent<NewEnemy>();
                    if (enemy == null) continue;

                    // Avoid charming the same enemy multiple times
                    if (!charmedEnemies.Contains(enemy))
                    {
                        enemy.ApplyCharm(abilityDuration);
                        charmedEnemies.Add(enemy);
                    }
                }
            }

            // After detection window, stop showing the aura
            if (total >= detectionDuration && charmInstance != null && charmInstance.isPlaying)
            {
                charmInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }

            total += Time.deltaTime;
            yield return null;
        }

        if (charmInstance != null)
            charmInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

}
