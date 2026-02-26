using UnityEngine;
using TMPro;

public class GemsView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gemsText;

    void Start()
    {
        GemManager.Instance.OnGemsChanged += UpdateGems;
        UpdateGems(GemManager.Instance.CurrentGems);
    }

    void UpdateGems(int amount)
    {
        gemsText.text = amount.ToString();
    }

    void OnDestroy()
    {
        if (GemManager.Instance != null)
            GemManager.Instance.OnGemsChanged -= UpdateGems;
    }
}