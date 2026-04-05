using UnityEngine;

public class BossHUDController : MonoBehaviour
{
    [Header("Boss & Views")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private HealthBarView healthBarView;
    [SerializeField] private HealthTextView healthTextView;

    void Update()
    {
        if (bossHealth == null)
        {
            bossHealth = FindFirstObjectByType<BossHealth>();
            if (bossHealth == null) return;
        }

        healthBarView.SetHealth(bossHealth.CurrentHealth, bossHealth.MaxHealth);
        healthTextView.SetHealth(bossHealth.CurrentHealth, bossHealth.MaxHealth);
    }
}

