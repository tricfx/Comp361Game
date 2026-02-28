using UnityEngine;
using System.Collections;

public class Heal : MonoBehaviour, IAbility
{
    [SerializeField] private int healPerSecond = 1;
    [SerializeField] private float duration = 10f;
    [SerializeField] private PlayerHealth playerHealth;
    public ParticleSystem healPrefab;   
    private ParticleSystem healInstance;


    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
        Vector3 spawnPosition = playerHealth.transform.position;

        healInstance = Instantiate(healPrefab, spawnPosition, Quaternion.identity);
    }

    public void Do()
    {
        if (playerHealth == null) return;

        if (healInstance != null)
        {
            healInstance.Play();
            Debug.Log("Heal effect started!");
        }
            

        Debug.Log("Healing started!");
        StartCoroutine(HealOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (healInstance != null)
            healInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator HealOverTime()
    {
        float total = 0f;
        float tick = 0f;

        while (total < duration && playerHealth.CurrentHealth < playerHealth.MaxHealth)
        {
            if (healInstance != null)
                healInstance.transform.position = playerHealth.transform.position;

            total += Time.deltaTime;
            tick += Time.deltaTime;

            if (tick >= 1f)
            {
                int ticks = Mathf.FloorToInt(tick);
                tick -= ticks;

                int healAmount = healPerSecond * ticks;
                playerHealth.Heal(healAmount);
            }

            yield return null;
        }

        if (healInstance != null)
            healInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
