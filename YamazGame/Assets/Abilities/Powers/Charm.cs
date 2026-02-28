using UnityEngine;
using System.Collections;

public class Charm : MonoBehaviour, IAbility
{
    [SerializeField] private float duration = 15f;
    [SerializeField] private PlayerHealth playerHealth;
    public ParticleSystem charm;
    private ParticleSystem charmInstance;



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

        while (total < duration)
        {
            if (charmInstance != null)
                charmInstance.transform.position = playerHealth.transform.position;

            total += Time.deltaTime;
            yield return null; // wait one frame
        }

        if (charmInstance != null)
            charmInstance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
