using UnityEngine;

public class ArenaTrigger : MonoBehaviour
{
    [SerializeField] public BossRoomController controller;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            controller.PlayerEnteredArena();
            // Destroy this trigger so it doesn't fire again
            gameObject.SetActive(false); 
        }
    }
}