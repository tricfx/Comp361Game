using UnityEngine;

public interface IDamageable {
    
    public float MaxHealth { set; get; }
    public float CurrentHealth { set; get; }
    public bool Targetable { set; get; }
    public bool Invincible { set; get; }
    public void OnHit(float damage, Vector2 knockback);
    public void OnHit(float damage);
    public void OnObjectDestroyed();

}