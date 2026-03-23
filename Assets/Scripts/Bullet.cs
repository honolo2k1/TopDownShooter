using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int bulletDamage;
    private float impactForce;

    private BoxCollider cd;
    private Rigidbody rb;
    private TrailRenderer trailRenderer;
    private MeshRenderer meshRenderer;

    [SerializeField] private GameObject bulletImpactFX;
    private Vector3 startPosition;
    private float flyDistance;
    private bool bulletDisable;

    private LayerMask allyLayerMask;

    protected virtual void Awake()
    {
        cd = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
    }
    protected virtual void Update()
    {
        FadeTrailIfNeeded();
        DisableIfNeeded();
        ReturnToPoolIfNeeded();

    }

    protected void ReturnToPoolIfNeeded()
    {
        if (trailRenderer.time < 0)
        {
            ReturnBulletToPool();
        }
    }

    protected void DisableIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance && !bulletDisable)
        {
            cd.enabled = false;
            meshRenderer.enabled = false;
            bulletDisable = true;
        }
    }

    protected void FadeTrailIfNeeded()
    {
        if (Vector3.Distance(startPosition, transform.position) > flyDistance - 1.5f)
        {
            trailRenderer.time -= 2 * Time.deltaTime;
        }
    }

    public void BulletSetup(LayerMask allyLayerMask, int bulletDamage, float flyDistance = 100, float impactForce = 100)
    {
        this.impactForce = impactForce;
        this.allyLayerMask = allyLayerMask;
        this.bulletDamage = bulletDamage;

        bulletDisable = false;
        cd.enabled = true;
        meshRenderer.enabled = true;
        trailRenderer.Clear();
        trailRenderer.time = 0.25f;
        startPosition = transform.position;
        this.flyDistance = flyDistance + 0.5f;
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
        //rb.constraints = RigidbodyConstraints.FreezeAll;

        if (!FriendlyFare())
        {
            if ((allyLayerMask.value & (1 << collision.gameObject.layer)) > 0)
            {
                ReturnBulletToPool(3);
                return;
            }
        }

        CreateImpactFX();
        ReturnBulletToPool();

        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(bulletDamage);

        ApplyImpactBulletToEnemy(collision);
    }

    private void ApplyImpactBulletToEnemy(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponentInParent<Enemy>();
        if (enemy != null)
        {
            Vector3 force = rb.linearVelocity.normalized * impactForce;
            Rigidbody hitRigidbody = collision.collider.attachedRigidbody;

            enemy.BulletImpact(force, collision.contacts[0].point, hitRigidbody);
        }
    }

    protected void ReturnBulletToPool(float delay = 0)
    {
        ObjectPool.Instance.ReturnObject(this.gameObject, delay);
    }

    protected void CreateImpactFX()
    {
        if (bulletImpactFX == null) return;

        GameObject newImpactFx = ObjectPool.Instance.GetObject(bulletImpactFX, transform);
        ObjectPool.Instance.ReturnObject(newImpactFx, 1);
    }

    public bool FriendlyFare() => GameManager.Instance.FriendlyFire;
}
