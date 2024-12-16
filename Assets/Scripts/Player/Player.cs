using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform playerBody;
    public PlayerControls controls { get; private set; }
    public PlayerAim aim { get; private set; }
    public PlayerMovement movement { get; private set; }
    public PlayerWeaponController weapon { get; private set; }
    public PlayerWeaponVisuals weaponVisuals { get; private set; }
    public PlayerInteraction interaction { get; private set; }
    public PlayerHealth health { get; private set; }
    public Ragdoll ragdoll { get; private set; }
    public Animator anim { get; private set; }
    public PlayerSoundFX sound { get; private set; }
    public bool controlsEnable { get; private set; }

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        ragdoll = GetComponent<Ragdoll>();
        anim = GetComponentInChildren<Animator>();
        aim = GetComponent<PlayerAim>();
        movement = GetComponent<PlayerMovement>();
        weapon = GetComponent<PlayerWeaponController>();
        weaponVisuals = GetComponent<PlayerWeaponVisuals>();
        interaction = GetComponent<PlayerInteraction>();
        controls = ControlsManager.Instance.controls;
        sound = GetComponent<PlayerSoundFX>();
    }
    private void OnEnable()
    {
        controls.Enable();

        controls.Character.UIMissionTooltipSwitch.performed += context => UI.Instance.InGameUI.SwitchMissionTooltip();
        controls.Character.UIPause.performed += context => UI.Instance.PauseSwitch();

    }
    private void OnDisable()
    {
        controls.Disable();
    }

    public void SetControlsEnableTo(bool enabled)
    {
        controlsEnable = enabled;
        ragdoll.CollidersActive(enabled);
        aim.EnableAimLaser(enabled);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            health.CurrentHealth = 999999;
            UI.Instance.InGameUI.UpdateHeathUI(health.CurrentHealth, health.MaxHealth);


            Car_HealthController car = FindObjectOfType<Car_HealthController>(true);
            if (car != null)
            {
                car.CurrentHealth = 99999;
                car.UpdateCarHealthUI();
            }

            weapon.CurrentWeapon().TotalReverseAmmo += 999999;
            weapon.CurrentWeapon().BulletsInMagazine = weapon.CurrentWeapon().TotalReverseAmmo;
            UI.Instance.InGameUI.UpdateWeaponUI(weapon.weaponSlots, weapon.currentWeapon);
        }
    }
}
