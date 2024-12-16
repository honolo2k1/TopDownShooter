using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car_HealthController : MonoBehaviour, IDamagable
{
    private Car_Controller carController;

    public int MaxHealth;
    public int CurrentHealth;

    private bool carBroken;

    [Header("Explosion Info")]
    [SerializeField] private int explosionDamage = 350;
    [Space]
    [SerializeField] private float explosionRadius = 3;
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private float explosionForce = 7f;
    [SerializeField] private float explosionUpwardsModifer = 2f;

    [SerializeField] private Transform explosionPoint;
    [Space]
    [SerializeField] private ParticleSystem fireFx;
    [SerializeField] private ParticleSystem explosionFx;
    private void Start()
    {
        carController = GetComponent<Car_Controller>();
        CurrentHealth = MaxHealth;
    }

    private void Update()
    {
        if (fireFx.gameObject.activeSelf)
        {
            fireFx.transform.rotation = Quaternion.identity;
        }
    }
    public void UpdateCarHealthUI()
    {
        UI.Instance.InGameUI.UpdateCarHealthUI(CurrentHealth, MaxHealth);
    }

    private void ReduceHealth(int damage)
    {
        if (carBroken)
            return;

        CurrentHealth -= damage;

        if (CurrentHealth < 0)
            BrakeTheCar();
    }

    private void BrakeTheCar()
    {
        carBroken = true;
        carController.BrakeTheCar();

        fireFx.gameObject.SetActive(true);
        StartCoroutine(ExplosionCo(explosionDelay));
    }

    public void TakeDamage(int damage)
    {
        ReduceHealth(damage);
        UpdateCarHealthUI();
    }

    private IEnumerator ExplosionCo(float delay)
    {
        yield return new WaitForSeconds(delay);

        explosionFx.gameObject.SetActive(true);
        carController.Rb.
            AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsModifer, ForceMode.Impulse);

        Explode();

        bool isGameCarDeliveryOver = gameObject.GetComponent<MissionObject_CarDeliver>();

        if (isGameCarDeliveryOver)
        {
            UI.Instance.ShowGameOverUI();
        }
    }

    private void Explode()
    {
        HashSet<GameObject> uniqeqEntites = new HashSet<GameObject>();

        Collider[] colliders = Physics.OverlapSphere(explosionPoint.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            IDamagable damageable = hit.GetComponent<IDamagable>();

            if (damageable != null)
            {
                GameObject rootEntity = hit.transform.root.gameObject;

                if (!uniqeqEntites.Add(rootEntity))
                {
                    continue;
                }

                damageable.TakeDamage(explosionDamage);

                hit.GetComponentInChildren<Rigidbody>().AddExplosionForce(explosionForce, explosionPoint.position, explosionRadius, explosionUpwardsModifer, ForceMode.VelocityChange);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(explosionPoint.position, explosionRadius);
    }
}
