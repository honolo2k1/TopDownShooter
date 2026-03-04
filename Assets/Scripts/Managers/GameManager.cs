using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Camera CameraForAim;
    [SerializeField] private Camera MainCamera;
    public Player Player;
    public bool IsCursorOn;

    [Header("Settings")]
    public bool FriendlyFire;
    [Space]
    public bool QuickStart;

    protected override void Initiate()
    {
        Player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        CameraForAim.fieldOfView = MainCamera.fieldOfView;
        IsCursorOn = Cursor.visible;
    }
    public void GameStart()
    {
        SetDefaultWeaponsForPlayer();
        //LevelGenarator.Instance.InitGeneration();
    }

    public void RestartScene() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public void GameCompleted()
    {
        UI.Instance.ShowVictoryScreenUI();

        ControlsManager.Instance.controls.Character.Disable();
        Player.Health.CurrentHealth += 99999;
    }
    public void GameOver()
    {
        TimeManager.Instance.SlowMotion(1.5f);
        UI.Instance.ShowGameOverUI();
        CameraManager.Instance.ChangeCameraDistance(4);
    }
    public void SetDefaultWeaponsForPlayer()
    {
        List<Weapon_Data> newList = UI.Instance.WeaponSelection.SelectedWeaponData();
        Player.Weapon.SetDefaultWeapon(newList);
    }
}
