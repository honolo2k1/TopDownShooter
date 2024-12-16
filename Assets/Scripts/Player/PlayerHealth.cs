public class PlayerHealth : HealthController
{
    private Player player;
    public bool IsDead { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);
        if (ShouldDie())
            Die();

        UI.Instance.InGameUI.UpdateHeathUI(CurrentHealth, MaxHealth);
    }
    private void Die()
    {
        if (IsDead)
            { return; }

        IsDead = true;
        player.anim.enabled = false;
        player.ragdoll.RagdollActive(false);

        GameManager.Instance.GameOver();
    }
}
