using UnityEngine;

public class IceMagic : MonoBehaviour, IAbility
{
    [SerializeField] private ParticleSystem iceEffect;
    [SerializeField] private GameObject player;

    public void Do()
    {
        // TODO: implement ice magic ability
        if (iceEffect != null)
            iceEffect.Play();
    }
}
