using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float zPosition = -1f;

    private void LateUpdate()
    {
        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            zPosition
        );
    }
}