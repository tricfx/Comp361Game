using UnityEngine;
using TMPro;

public class HealthTextView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    public void SetHealth(int current, int max)
    {
        if (text == null) return;
        text.text = current + " / " + max;
    }
}