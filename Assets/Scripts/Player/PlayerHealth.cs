
using TMPro;
using UnityEngine;


public class PlayerHealth : HealthController
{
    private Player player;

    public GameObject FloatingTextPrefab;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    public override void ReduceHealth(int damage)
    {
        base.ReduceHealth(damage);

        if (FloatingTextPrefab != null)
        {
            ShowFloatingText(damage);
        }

        if (ShouldDie())
            Die();

        UI.Instance.InGameUI.UpdateHeathUI(CurrentHealth, MaxHealth);
    }

    private void ShowFloatingText(int damage)
    {
        var floatingText = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity, transform);
        var text = floatingText.GetComponent<TextMeshPro>();

        text.text = $"-{damage}";
    }
    private void Die()
    {
        if (IsDead)
        { return; }

        IsDead = true;
        player.Anim.enabled = false;
        player.Ragdoll.RagdollActive(false);

        GameManager.Instance.GameOver();
    }
}
