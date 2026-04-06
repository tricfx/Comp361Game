using UnityEngine;

public class BossRoomController : MonoBehaviour
{
    [SerializeField] private GameObject entranceWall;
    [SerializeField] private GameObject exitWall;    
    private bool bossFightActive = false;

    private void Update()
    {

    }

    public void PlayerEnteredArena()
    {
        if (!bossFightActive)
        {
            LockArena();
            bossFightActive = true;
        }
    }

    private void LockArena()
    {
        entranceWall.SetActive(true);
        exitWall.SetActive(true);
        Debug.Log("Boss fight Started. Doors closing");
    }

    public void UnlockArena()
    {
        entranceWall.SetActive(false);
        exitWall.SetActive(false);
        bossFightActive = false; 
        Debug.Log("Boss Slain. Doors opening");
    }
}