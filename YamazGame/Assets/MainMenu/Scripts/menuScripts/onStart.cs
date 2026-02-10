using UnityEngine;

public class onStart : MonoBehaviour
{
    [SerializeField] private GameObject welcomeScreen;
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject black;

    private void Awake()
    {
        // Disable everything in the canvas
        foreach (Transform child in canvas.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Enable welcome screen only
        welcomeScreen.SetActive(true);
        black.SetActive(true);

    }
}
