using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float duration = 0.15f;
    [SerializeField] private float magnitude = 0.2f;

    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    private void Awake()
    {
        originalPos = transform.localPosition;
        Debug.Log("CameraShake attached to: " + gameObject.name);
    }

    public void Shake()
    {
        Debug.Log("SHAKE CALLED");

        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake());
    }

    private IEnumerator DoShake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector2 offset = Random.insideUnitCircle * magnitude;
            transform.localPosition = originalPos + (Vector3)offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}