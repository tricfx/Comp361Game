using System.Runtime.InteropServices.WindowsRuntime;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;

public class DamageableCharacter : MonoBehaviour, IDamageable {

    public GameObject healthText;
    public bool disableSimulation = false;
    public bool canTurnInvincible = false;
    public float invincibilityLimit = 0.3f;
    Animator animator;
    Rigidbody2D rb;
    Collider2D physicsCollider;
    bool alive = true;
    private float invincibleTimeElapsed = 0f;

    public float MaxHealth {
        set {
            _maxHealth = value;
        }
        get {
            return _maxHealth;
        }
    }

    public float CurrentHealth {
        set {
            if (value < _currentHealth) {
                animator.SetTrigger("hit");
                RectTransform textTransform = Instantiate(healthText).GetComponent<RectTransform>();
                textTransform.transform.position = Camera.main.WorldToScreenPoint(gameObject.transform.position);

                Canvas canvas = GameObject.FindFirstObjectByType<Canvas>();
                textTransform.SetParent(canvas.transform);
            }

            _currentHealth = value;

            if (_currentHealth <= 0) {
                animator.SetBool("alive", false);
                Targetable = false;
            }
        }
        get {
            return _currentHealth;
        }
    }

    public bool Targetable {
        set {
            _targetable = value;
            if (disableSimulation) {
                rb.simulated = false;
            }
            physicsCollider.enabled = value;
        }
        get {
            return _targetable;
        }
    }

    public bool Invincible {
        set {
            _invincible = value;

            if (_invincible == true) {
                invincibleTimeElapsed = 0f;
            }
        }    
        get {
            return _invincible;
        }    
    }

    [SerializeField] float _maxHealth = 3f;
    float _currentHealth;
    bool _targetable = true;
    bool _invincible = false;

    void Start() {
        CurrentHealth = MaxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        physicsCollider = GetComponent<Collider2D>();
        animator.SetBool("alive", alive);
    }

    public void OnHit(float damage, Vector2 knockback) {
        if (!Invincible) {
            CurrentHealth -= damage;
            rb.AddForce(knockback, ForceMode2D.Impulse);
            
            if (canTurnInvincible) {
                Invincible = true;
            }
        }
    }

    public void OnHit(float damage) {
        if (!Invincible) {
            CurrentHealth -= damage;

            if (canTurnInvincible) {
                Invincible = true;
            }
        }
    }

    public void OnObjectDestroyed() {
        Destroy(gameObject);
    }

    public void FixedUpdate() {
        if (Invincible) {
            invincibleTimeElapsed += Time.deltaTime;

            if (invincibleTimeElapsed > invincibilityLimit) {
                Invincible = false;
            }
        }
    }
}
