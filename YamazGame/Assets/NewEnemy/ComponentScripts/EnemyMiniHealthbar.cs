using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyMiniHealthbar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private GameObject visualsRoot;
    [SerializeField] private bool hideWhenFull = true;
    [SerializeField] private float collapseDuration = 0.2f;

    private Coroutine collapseRoutine;
    private bool isCollapsed;

    public void SetNormalized(float value)
    {
        value = Mathf.Clamp01(value);
        fillImage.fillAmount = value;

        if (value <= 0f)
        {
            if (!isCollapsed)
            {
                if (collapseRoutine != null)
                    StopCoroutine(collapseRoutine);

                collapseRoutine = StartCoroutine(CollapseRoutine());
            }
            return;
        }

        if (collapseRoutine != null)
        {
            StopCoroutine(collapseRoutine);
            collapseRoutine = null;
        }

        isCollapsed = false;
        visualsRoot.transform.localScale = Vector3.one;

        if (hideWhenFull && value >= 0.999f)
            visualsRoot.SetActive(false);
        else
            visualsRoot.SetActive(true);
    }

    private IEnumerator CollapseRoutine()
    {
        isCollapsed = true;
        visualsRoot.SetActive(true);

        Vector3 startScale = visualsRoot.transform.localScale;
        Vector3 endScale = new Vector3(0f, startScale.y, startScale.z);

        float t = 0f;
        while (t < collapseDuration)
        {
            t += Time.deltaTime;
            visualsRoot.transform.localScale = Vector3.Lerp(startScale, endScale, t / collapseDuration);
            yield return null;
        }

        visualsRoot.transform.localScale = endScale;
        visualsRoot.SetActive(false);
        collapseRoutine = null;
    }
}