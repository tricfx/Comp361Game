using TMPro;
using UnityEngine;

public class ErrorPopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] screens;
    [SerializeField] private GameObject[] backgrounds;

    [SerializeField] private TMP_Text errorMsg;
    [SerializeField] private AudioSource sfx;

    public void Show(string msg)
    {
        sfx?.Play();
        errorMsg.text = msg;

        for (int i = 0; i < backgrounds.Length; i++)
            if (backgrounds[i] != null) backgrounds[i].SetActive(false);

        int bestIdx = 0;
        float bestAlpha = -1f;

        for (int i = 0; i < screens.Length; i++)
        {
            var cg = screens[i];
            if (cg == null) continue;
            if (!cg.gameObject.activeInHierarchy) continue;

            if (cg.alpha > bestAlpha)
            {
                bestAlpha = cg.alpha;
                bestIdx = i;
            }
        }

        if (bestIdx >= 0 && bestIdx < backgrounds.Length && backgrounds[bestIdx] != null)
            backgrounds[bestIdx].SetActive(true);

        gameObject.SetActive(true);
        transform.SetAsLastSibling();
    }

    public void Hide() => gameObject.SetActive(false);
}
