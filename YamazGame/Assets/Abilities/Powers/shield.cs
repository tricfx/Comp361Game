using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour, IAbility
{
    [SerializeField] private float duration = 5f;
    [SerializeField] private PlayerHealth playerHealth;
    public ParticleSystem shield;
    private ParticleSystem shieldInstance;



    private void Awake()
    {
        if (!playerHealth)
            playerHealth = GetComponentInParent<PlayerHealth>();

        Vector3 spawnPosition = playerHealth.transform.position;

        shieldInstance = Instantiate(shield, spawnPosition, Quaternion.identity);
    }

    public void Do()
    {

        if (shieldInstance != null)
        {
            shieldInstance.Play();
        }
        StartCoroutine(CharmOverTime());
    }

    public void Dispose()
    {
        StopAllCoroutines();
        if (shieldInstance != null)
            shieldInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Destroy(gameObject);
    }

    private IEnumerator CharmOverTime()
    {
        float total = 0f;

        while (total < duration)
        {
            if (shieldInstance != null)
                shieldInstance.transform.position = playerHealth.transform.position;

            total += Time.deltaTime;
            yield return null; // wait one frame
        }

        if (shieldInstance != null)
            shieldInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
