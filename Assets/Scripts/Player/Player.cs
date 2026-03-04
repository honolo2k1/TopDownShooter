using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls Controls { get; private set; }
    public PlayerAim Aim { get; private set; }
    public PlayerMovement Movement { get; private set; }
    public PlayerWeaponController Weapon { get; private set; }
    public PlayerWeaponVisuals WeaponVisuals { get; private set; }
    public PlayerInteraction Interaction { get; private set; }
    public PlayerHealth Health { get; private set; }
    public Ragdoll Ragdoll { get; private set; }
    public Animator Anim { get; private set; }
    public PlayerSoundFX Sound { get; private set; }
    public bool ControlsEnable { get; private set; }

    private void Awake()
    {
        Health = GetComponent<PlayerHealth>();
        Ragdoll = GetComponent<Ragdoll>();
        Anim = GetComponentInChildren<Animator>();
        Aim = GetComponent<PlayerAim>();
        Movement = GetComponent<PlayerMovement>();
        Weapon = GetComponent<PlayerWeaponController>();
        WeaponVisuals = GetComponent<PlayerWeaponVisuals>();
        Interaction = GetComponent<PlayerInteraction>();
        Controls = ControlsManager.Instance.controls;
        Sound = GetComponent<PlayerSoundFX>();
    }
    private void OnEnable()
    {
        Controls.Enable();

        Controls.Character.UIMissionTooltipSwitch.performed += context => UI.Instance.InGameUI.SwitchMissionTooltip();
        Controls.Character.UIPause.performed += context => UI.Instance.PauseSwitch();

    }
    private void OnDisable()
    {
        Controls.Disable();
    }

    public void SetControlsEnableTo(bool enabled)
    {
        ControlsEnable = enabled;
        Ragdoll.CollidersActive(enabled);
        Aim.EnableAimLaser(enabled);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Health.CurrentHealth = 999999;
            UI.Instance.InGameUI.UpdateHeathUI(Health.CurrentHealth, Health.MaxHealth);

            Car_HealthController car = Object.FindFirstObjectByType<Car_HealthController>(FindObjectsInactive.Include);
            if (car != null)
            {
                car.CurrentHealth = 99999;
                car.UpdateCarHealthUI();
            }

            Weapon.CurrentWeapon().TotalReverseAmmo += 999999;
            Weapon.CurrentWeapon().BulletsInMagazine = Weapon.CurrentWeapon().TotalReverseAmmo;
            UI.Instance.InGameUI.UpdateWeaponUI(Weapon.weaponSlots, Weapon.currentWeapon);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            var enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

            foreach (var enemy in enemies)
            {
                enemy.GetHit(100000);
            }
        }
    }
}
