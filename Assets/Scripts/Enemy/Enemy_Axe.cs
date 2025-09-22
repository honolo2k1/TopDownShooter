using UnityEngine;

public class Enemy_Axe : MonoBehaviour
{
    [SerializeField] private GameObject impactFx;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform axeVisual;

    private Transform player;
    private float flySpeed;
    private Vector3 dicrection;
    private float rotationSpeed;
    private float timer = 1;

    private int damage;

    private void Update()
    {
        axeVisual.Rotate(Vector3.right * rotationSpeed * Time.deltaTime);
        timer -= Time.deltaTime;

        if (timer > 0)
        {
            dicrection = player.position + Vector3.up - transform.position;
        }

        transform.forward = rb.linearVelocity;
    }
    private void FixedUpdate()
    {
        rb.linearVelocity = dicrection.normalized * flySpeed;
    }
    public void AxeSetup(float flySpeed, Transform player, float timer, int damage)
    {
        rotationSpeed = 1600;

        this.damage = damage;
        this.flySpeed = flySpeed;
        this.player = player;
        this.timer = timer;
    }
    private void OnCollisionEnter(Collision collision)
    {
        IDamagable damagable = collision.gameObject.GetComponent<IDamagable>();
        damagable?.TakeDamage(damage);

        GameObject newFx = ObjectPool.Instance.GetObject(impactFx, transform);

        ObjectPool.Instance.ReturnObject(gameObject);
        ObjectPool.Instance.ReturnObject(newFx, 1f);
    }
}
