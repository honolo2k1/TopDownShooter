using UnityEngine;
using UnityEngine.UI;

public class Enemy_Health : HealthController
{
    [SerializeField] private Image healthBar;
    public GameObject HealthBar;

    public GameObject FloatingTextPrefab;

    private Transform cachedCameraTransform;

    private void Start()
    {
        cachedCameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (cachedCameraTransform != null)
            HealthBar.transform.rotation = Quaternion.LookRotation(cachedCameraTransform.forward);
    }
    public void UpdateHeathUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
