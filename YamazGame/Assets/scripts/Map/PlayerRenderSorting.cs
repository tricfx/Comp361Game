using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerRenderSorting : MonoBehaviour
{
    [SerializeField] Transform feet; 
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        float y = feet != null ? feet.position.y : transform.position.y;
        sr.sortingOrder = Mathf.FloorToInt(-y * 100);
    }
}