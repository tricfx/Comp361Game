using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour {

    public string tagTarget = "Player";
    public List<Collider2D> detectedObjects = new List<Collider2D>();
    public Collider2D detectionCollider;

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == tagTarget) detectedObjects.Add(other);
    }

    void OnTriggerExit2D(Collider2D other) {
        detectedObjects.Remove(other);
    }
}
