using System.Collections.Generic;
using UnityEngine;

public class AttackZone : MonoBehaviour {
    
    public string tagTarget = "Player";
    bool playerInRange;
    Enemy enemy;

    void Start() {
        enemy = GetComponentInParent<Enemy>();
    }

    void Update() {
        if (playerInRange) enemy.Attack();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == tagTarget) playerInRange = true;
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.gameObject.tag == tagTarget) playerInRange = false;
    }

}
