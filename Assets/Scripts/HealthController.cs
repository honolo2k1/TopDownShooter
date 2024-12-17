using UnityEngine;

public class HealthController : MonoBehaviour
{
    public int MaxHealth;
    public int CurrentHealth;

    public bool IsDead;
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
        if (IsDead)
        {
            return false;
        }
        if (CurrentHealth < 0)
        {
            IsDead = true;
            return true;
        }
        return false;
    }
}
