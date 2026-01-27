using UnityEngine;

public class BackSPace : MonoBehaviour
{
    public GameObject panelToClose;
    public GameObject panelToOpen;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            panelToClose.SetActive(false);
            panelToOpen.SetActive(true);
        }
    }
}
