using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollParent;

    private Collider[] ragdollColliders;
    private Rigidbody[] ragdollRigidbodies;

    private void Awake()
    {
        ragdollColliders = GetComponentsInChildren<Collider>();
        ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();

        RagdollActive(true);

        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }

    public void RagdollActive(bool isActive)
    {
        foreach (Rigidbody rb in ragdollRigidbodies)
        {
            rb.isKinematic = isActive;
        }
    }

    public void CollidersActive(bool isActive)
    {
        foreach (Collider cd in ragdollColliders)
        {
            cd.enabled = isActive;
        }
    }
}
