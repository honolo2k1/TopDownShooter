using UnityEngine;

public class Dummy : MonoBehaviour, IDamagable
{
    public int CurrentHealth;
    public int MaxHealth = 100;
    [Space]
    public MeshRenderer Mesh;
    public Material WhiteMat;
    public Material RedMat;
    [Space]
    public float RefreshCooldown;
    private float lastTimeDamage;
    private void Start() => Refresh();
    private void Update()
    {
        if (Time.time > lastTimeDamage + RefreshCooldown || Input.GetKeyDown(KeyCode.B))
        {
            Refresh();
        }
    }
    private void Refresh()
    {
        CurrentHealth = MaxHealth;
        Mesh.sharedMaterial = WhiteMat;
    }
    public void TakeDamage(int damage)
    {
        lastTimeDamage = Time.time;
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Mesh.sharedMaterial = RedMat;
    }
}