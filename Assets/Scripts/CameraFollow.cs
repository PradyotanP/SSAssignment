using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // The object (typically player) the camera will follow

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 5f, -10f); // Offset from the target's position
    public float followSpeed = 5f; // Speed at which the camera follows the target
    public float rotationSmoothTime = 0.1f; // Speed of camera rotation smoothing

    private Vector3 currentVelocity; // Used by SmoothDamp to store the velocity reference

    void LateUpdate()
    {
        // Ensure a target has been assigned
        if (!target) return;

        // Desired camera position based on target and offset
        Vector3 targetPosition = target.position + offset;

        // Smoothly interpolate camera's position toward target position
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / followSpeed
        );

        // Calculate the direction from camera to target for smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);

        // Smoothly rotate the camera to look at the target
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothTime
        );
    }
}
