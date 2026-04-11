using UnityEngine;
using UnityEngine.UI;

public class DisableButtonNavigation : MonoBehaviour
{
    private void Awake()
    {
        ApplyToAllSelectablesInScene();
    }

    [ContextMenu("Apply To All Selectables In Scene")]
    public void ApplyToAllSelectablesInScene()
    {
        Selectable[] selectables = FindObjectsByType<Selectable>(FindObjectsSortMode.None);

        foreach (Selectable selectable in selectables)
        {
            Navigation nav = selectable.navigation;
            nav.mode = Navigation.Mode.None;
            selectable.navigation = nav;
        }
    }
}