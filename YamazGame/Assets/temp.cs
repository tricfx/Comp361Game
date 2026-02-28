using UnityEngine;

public class temp : MonoBehaviour
{
    public ParticleSystem healPrefab;   // drag prefab from Assets here
    private ParticleSystem healInstance;

    void Start()
    {
        healInstance = Instantiate(healPrefab, new Vector3(8.3f, 43.8f, 0f), Quaternion.identity);
        healInstance.Play();
    }
}