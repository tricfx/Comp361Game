using UnityEngine;

public class LevelCompleteListener : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private string triggerName = "Activate";

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EnemyManager.OnAllEnemiesDefeated += OnLevelComplete;
    }

    private void OnDisable()
    {
        EnemyManager.OnAllEnemiesDefeated -= OnLevelComplete;
    }

    private void OnLevelComplete()
    {
        Debug.Log($"{gameObject.name} activated because all enemies are defeated");

        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }
    }
}