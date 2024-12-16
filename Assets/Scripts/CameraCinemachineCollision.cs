using UnityEngine;

public class CameraCinemachineCollision : MonoBehaviour
{
    public Transform cameraFollowTarget;  // The object the camera is following
    public LayerMask buildingLayer;       // Set this to the Building layer in the Inspector
    public float collisionRadius = 0.5f;  // Radius for the collision check
    public float collisionOffset = 0.1f;  // Offset distance to prevent sticking to walls

    private Vector3 lastValidPosition;    // Last known valid position of the follow target

    void Start()
    {
        lastValidPosition = cameraFollowTarget.position; // Store initial valid position
    }

    void Update()
    {
        PreventCameraTargetFromColliding();
    }

    private void PreventCameraTargetFromColliding()
    {
        RaycastHit hit;
        Vector3 direction = (cameraFollowTarget.position - transform.position).normalized;

        // Use SphereCast to check for collisions
        if (Physics.SphereCast(transform.position, collisionRadius, direction, out hit, collisionRadius, buildingLayer))
        {
            // Smoothly move the target back to the last valid position
            Vector3 collisionSafePosition = hit.point + hit.normal * collisionOffset;
            cameraFollowTarget.position = Vector3.Lerp(cameraFollowTarget.position, collisionSafePosition, Time.deltaTime * 5f);
        }
        else
        {
            // Update last valid position if no collision is detected
            lastValidPosition = cameraFollowTarget.position;
        }
    }


    private void OnDrawGizmosSelected()
    {
        // Optional: Visualize the collision check in the scene view
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(cameraFollowTarget.position, collisionRadius);
    }
}
