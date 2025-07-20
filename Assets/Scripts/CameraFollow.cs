using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 5f, -10f);
    public float followSpeed = 5f;
    public float rotationSmoothTime = 0.1f;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref currentVelocity, 1f / followSpeed);

        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }
}
