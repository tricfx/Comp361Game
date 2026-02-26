using UnityEngine;
using System.Collections;

public class Heal : MonoBehaviour, IAbility
{
    [SerializeField] private int healPerSecond = 1;
    [SerializeField] private float duration = 10f;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private ParticleSystem healEffect;

    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();
    }

    public void Do()
    {
        if (playerHealth == null) return;

        if (healEffect != null)
            healEffect.Play();

        Debug.Log("Healing started!");
        StartCoroutine(HealOverTime());
    }

    private IEnumerator HealOverTime()
    {
        float total = 0f;
        float tick = 0f;

        while (total < duration && playerHealth.CurrentHealth < playerHealth.MaxHealth)
        {
            if (healEffect != null)
                healEffect.transform.position = playerHealth.transform.position;

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

        if (healEffect != null)
            healEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
