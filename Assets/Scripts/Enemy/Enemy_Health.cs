using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_Health : HealthController
{
    [SerializeField] private Image healthBar;
    public GameObject HealthBar;

    public GameObject FloatingTextPrefab;
    private void Update()
    {
        HealthBar.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
    }
    public void UpdateHeathUI(float currentHealth, float maxHealth)
    {
        healthBar.fillAmount = currentHealth / maxHealth;
    }
}
