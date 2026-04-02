using UnityEngine;

public class MinimapPlayerArrow : MonoBehaviour
{
    [SerializeField] private PlayerController2D playerController;
    [SerializeField] private float rotateSpeed;

    private float angleOffset = -90f;

    private void LateUpdate()
    {
        Vector2 dir = playerController.MinimapDir;
        if (dir.sqrMagnitude < 0.001f) return;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        float currentAngle = transform.localEulerAngles.z;

        float newAngle = Mathf.MoveTowardsAngle(
            currentAngle,
            targetAngle,
            rotateSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
    }
}