using UnityEngine;

public class GongHammerEventRelay : MonoBehaviour
{
    [SerializeField] private ShopShuffleButton shopShuffleButton;

    public void ApplyReroll()
    {
        shopShuffleButton.ApplyReroll();
    }
}