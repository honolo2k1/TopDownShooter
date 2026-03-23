using System.Collections.Generic;
using UnityEngine;

public class Enemy_Grenade : MonoBehaviour
{
    [SerializeField] private GameObject explosionFx;
    [SerializeField] private float impactRadius;
    [SerializeField] private float upwardsMultiplier = 1;
    private Rigidbody rb;
    private float timer;
    private float impactPower;

    private LayerMask allyLayerMask;
    private bool canExplode = true;
    private int grenadeDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer < 0 && canExplode)
        {
            Explode();
        }
    }

    private void Explode()
    {
        canExplode = false;

        HashSet<GameObject> uniqueEntities = new HashSet<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(transform.position, impactRadius);
        foreach (Collider hit in colliders)
        {
            IDamagable damagable = hit.GetComponent<IDamagable>();
            if (damagable != null)
            {
                if (!IsTargetValid(hit))
                {
                    continue;
                }
                GameObject rootEntity = hit.transform.root.gameObject;
                if (!uniqueEntities.Add(rootEntity))
                {
                    continue;
                }

                damagable.TakeDamage(grenadeDamage);
            }

            ApplyPhysicalForceTo(hit);
        }

        PlayExplosionFx();
        ObjectPool.Instance.ReturnObject(gameObject);
    }

    private void ApplyPhysicalForceTo(Collider hit)
    {
        Rigidbody hitRb = hit.GetComponent<Rigidbody>();
        if (hitRb != null)
            hitRb.AddExplosionForce(impactPower, transform.position, impactRadius, upwardsMultiplier, ForceMode.Impulse);
    }

    private void PlayExplosionFx()
    {
        GameObject newFX = ObjectPool.Instance.GetObject(explosionFx, transform);
        ObjectPool.Instance.ReturnObject(newFX, 1);
    }

    public void SetupGrenade(LayerMask allyLayerMask, Vector3 target, float timeToTarget, float countdown, float impactPower, int grenadeDamage)
    {
        canExplode = true;

        this.grenadeDamage = grenadeDamage;
        this.allyLayerMask = allyLayerMask;
        this.impactPower = impactPower;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.linearVelocity = CalculateLaunchVelocity(target, timeToTarget);
        timer = countdown + timeToTarget;
    }

    private bool IsTargetValid(Collider collider)
    {
        if (GameManager.Instance.FriendlyFire)
            return true;

        if ((allyLayerMask.value & (1 << collider.gameObject.layer)) > 0)
            return false;

        return true;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 target, float timeToTarget)
    {
        Vector3 direction = target - transform.position;
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z);

        Vector3 velocityXZ = directionXZ / timeToTarget;

        float velocityY = (direction.y - (Physics.gravity.y * Mathf.Pow(timeToTarget, 2)) / 2) / timeToTarget;

        Vector3 launchVelocity = velocityXZ + Vector3.up * velocityY;

        return launchVelocity;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }
}
