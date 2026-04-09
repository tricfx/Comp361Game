using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour, IAbility
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Vector3 shieldOffset;
    public ParticleSystem shield;
    private ParticleSystem shieldInstance;

    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();

        shieldInstance = Instantiate(shield, playerHealth.transform.position + shieldOffset, Quaternion.identity);
    }

    public void Do()
    {
        if (shieldInstance != null)
            shieldInstance.Play();

        StartCoroutine(ShieldRoutine());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (playerHealth != null)
            playerHealth.isInvincible = false;
        if (shieldInstance != null)
            shieldInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator ShieldRoutine()
    {
        playerHealth.isInvincible = true;

        float total = 0f;
        while (total < duration)
        {
            if (shieldInstance != null)
                shieldInstance.transform.position = playerHealth.transform.position + shieldOffset;

            total += Time.deltaTime;
            yield return null;
        }

        playerHealth.isInvincible = false;

        if (shieldInstance != null)
            shieldInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
