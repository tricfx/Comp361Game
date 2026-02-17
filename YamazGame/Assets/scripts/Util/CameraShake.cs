using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    private CinemachineCamera vcam;
    private CinemachineBasicMultiChannelPerlin noise;

    void Awake()
    {
        // Try to find a CinemachineCamera anywhere in the scene
        vcam = GetComponent<CinemachineCamera>();

        if (vcam == null)
            vcam = GetComponentInChildren<CinemachineCamera>(true);

        if (vcam == null)
            vcam = Object.FindFirstObjectByType<CinemachineCamera>(FindObjectsInactive.Include);

        // Try to find the Perlin noise on the camera or its children
        if (vcam != null)
        {
            noise = vcam.GetComponent<CinemachineBasicMultiChannelPerlin>();

            if (noise == null)
                noise = vcam.GetComponentInChildren<CinemachineBasicMultiChannelPerlin>(true);
        }

        Debug.Log("CameraShake Awake on " + gameObject.name + ", vcam=" + (vcam!=null) + ", noise=" + (noise!=null));

        if (noise != null)
        {
            noise.AmplitudeGain = 0f; // ensure no shake at start
        }
        else
        {
            Debug.LogWarning("CameraShake: No Cinemachine noise component found in scene!");
        }
    }

    public void Shake(float duration = 0.2f, float strength = 0.5f)
    {
        Debug.Log("CameraShake.Shake() called on " + gameObject.name);
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, strength));
    }

    IEnumerator ShakeRoutine(float duration, float strength)
    {
        if (noise == null)
            yield break;

        noise.AmplitudeGain = strength;

        yield return new WaitForSeconds(duration);

        noise.AmplitudeGain = 0f; // stop shaking
    }
}