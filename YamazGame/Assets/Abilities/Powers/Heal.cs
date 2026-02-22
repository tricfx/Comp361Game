using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Heal : MonoBehaviour , IAbility
{
    [SerializeField]
    private int healPerSecond = 1;

    [SerializeField]
    private float duration = 10f;

    [SerializeField]
    private Player player;

    [SerializeField]
    private ParticleSystem healEffect;

    public void Do()
    {
        if (player == null) return;

        // Play particle effect
        if (healEffect != null)
            healEffect.Play();
        Debug.Log("Healing started!");
        StartCoroutine(HealOverTime());
    }

    private IEnumerator HealOverTime()
    {
        float total = 0f;
        float tick = 0f;

        if (healEffect != null)
            healEffect.Play();

        while (total < duration && player.currentHP < player.maxHP)
        {
            // Follow every frame
            if (healEffect != null)
                healEffect.transform.position = player.transform.position;

            total += Time.deltaTime;
            tick += Time.deltaTime;

            // Heal once per second
            if (tick >= 1f)
            {
                int ticks = Mathf.FloorToInt(tick);   // handles lag spikes
                tick -= ticks;

                int healAmount = healPerSecond * ticks;
                player.currentHP = Mathf.Min(player.currentHP + healAmount, player.maxHP);
            }

            yield return null;
        }

        if (healEffect != null)
            healEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }



}
