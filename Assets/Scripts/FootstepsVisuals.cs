using UnityEngine;

public class FootstepsVisuals : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private TrailRenderer leftFoot;
    [SerializeField] private TrailRenderer rightFoot;

    [Range(0.001f, 0.3f)]
    [SerializeField] private float checkRadius = 0.05f;
    [Range(-0.15f, 0.15f)]
    [SerializeField] private float rayDistance = -0.05f;

    private int frameOffset;

    private void Start()
    {
        // Randomize offset so not all enemies check on the same frame
        frameOffset = Random.Range(0, 4);
    }

    private void Update()
    {
        // Throttle: only check every 4 frames
        if ((Time.frameCount + frameOffset) % 4 != 0)
            return;

        CheckFootSteps(leftFoot);
        CheckFootSteps(rightFoot);
    }
    private void CheckFootSteps(TrailRenderer foot)
    {
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        bool touchingGround = Physics.CheckSphere(checkPosition, checkRadius, whatIsGround);

        foot.emitting = touchingGround;
    }
    private void OnDrawGizmos()
    {
        DrawFootGizmos(leftFoot.transform);
        DrawFootGizmos(rightFoot.transform);
    }

    private void DrawFootGizmos(Transform foot)
    {
        if (foot == null)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Vector3 checkPosition = foot.transform.position + Vector3.down * rayDistance;

        Gizmos.DrawWireSphere(checkPosition, checkRadius);
    }
}
