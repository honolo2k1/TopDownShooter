using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    private bool isDead;
    protected virtual void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public virtual void ReduceHealth(int damage)
    {
        CurrentHealth -= damage;
    }
    public virtual void IncreaseHealth()
    {
        CurrentHealth++;
        if (CurrentHealth > MaxHealth)
        {
            CurrentHealth = MaxHealth;
        }
    }
    public bool ShouldDie()
    {
        if (isDead)
        {
            return false;
        }
        if (CurrentHealth < 0)
        {
            isDead = true;
            return true;
        }
        return false;
    }
}
