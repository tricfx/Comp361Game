using UnityEngine;
using UnityEngine.UI;

public class HealthBarView : MonoBehaviour
{                     
    [SerializeField] private Image fillImage;  

    public void SetHealth(int current, int max)
    {
        if (fillImage == null) return;
        if (max <= 0) return;

        fillImage.fillAmount = Mathf.Clamp01((float)current / max);
    }
}