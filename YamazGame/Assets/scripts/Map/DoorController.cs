using System.Diagnostics;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    private Animator animator;
    private Collider2D doorCollider;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        doorCollider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        EnemyManager.OnAllEnemiesDefeated += OpenDoor;
    }

    private void OnDisable()
    {
        EnemyManager.OnAllEnemiesDefeated -= OpenDoor;
    }

    public void OpenDoor()
    {
        UnityEngine.Debug.Log("Door Opening");
        animator.SetTrigger("Open");
        doorCollider.enabled = false;
    }
}
