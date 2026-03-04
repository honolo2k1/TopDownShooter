using UnityEngine;

public class ControlsManager : MonoSingleton<ControlsManager>
{
    public PlayerControls controls { get; private set; }
    private Player player;

    protected override void Initiate()
    {
        controls = new PlayerControls();
    }
    private void Start()
    {
        player = GameManager.Instance.Player;

        //SwitchToCharacterControls();
    }

    public void SwitchToCharacterControls()
    {
        Cursor.visible = false;

        controls.UI.Disable();

        controls.Car.Disable();
        controls.Character.Enable();
        player.SetControlsEnableTo(true);
        player.Aim.aim.GetComponent<SpriteRenderer>().enabled = true;

        UI.Instance.InGameUI.SwitchToCharcaterUI();
    }

    public void SwitchToUIControls()
    {
        Cursor.visible = true;

        controls.UI.Enable();

        controls.Car.Disable();
        controls.Character.Disable();
        player.SetControlsEnableTo(false);

        player.Aim.aim.GetComponent<SpriteRenderer>().enabled = false;
    }

    public void SwitchToCarControls()
    {
        controls.Car.Enable();

        controls.UI.Disable();
        controls.Character.Disable();
        player.SetControlsEnableTo(false);

        UI.Instance.InGameUI.SwitchToCarUI();

        player.Aim.aim.GetComponent<SpriteRenderer>().enabled = false;
    }
}
