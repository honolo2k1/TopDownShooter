using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public UI_InGame InGameUI { get; private set; }
    public UI_WeaponSelection WeaponSelection { get; private set; }
    public UI_GameOver GameOverUI { get; private set; }
    public UI_Settings SettingsUI { get; private set; }

    public GameObject VictoryScreenUI;
    public GameObject PauseUI;

    [SerializeField] GameObject[] UIElements;

    [Header("Fade Image")]
    [SerializeField] private Image fadeImage;

    private void Awake()
    {
        Instance = this;
        InGameUI = GetComponentInChildren<UI_InGame>(true);
        WeaponSelection = GetComponentInChildren<UI_WeaponSelection>(true);
        GameOverUI = GetComponentInChildren<UI_GameOver>(true);
        SettingsUI = GetComponentInChildren<UI_Settings>(true);

        Cursor.visible = true;
    }
    private void Start()
    {
        AssignInputsUI();

        StartCoroutine(ChangeImageAlpha(0, 1.5f, null));

        if (GameManager.Instance.QuickStart)
        {
            LevelGenarator.Instance.InitGeneration();
            StartTheGame();
        }
    }
    public void SwitchToUI(GameObject uiToSwitchOn)
    {
        foreach (var element in UIElements)
        {
            element.SetActive(false);
        }

        uiToSwitchOn.SetActive(true);

        if (uiToSwitchOn == SettingsUI.gameObject)
        {
            SettingsUI.LoadSettings();
        }

        if (uiToSwitchOn == InGameUI.gameObject)
        {
            ControlsManager.Instance.SwitchToCharacterControls();
        }
    }
    public void StartTheGame() => StartCoroutine(StartGameSequence());
    public void QuitTheGame() => Application.Quit();
    public void StartLevelGeneration() => LevelGenarator.Instance.InitGeneration();

    public void RestartTheGame()
    {
        TimeManager.Instance.ResumeTime();
        StartCoroutine(ChangeImageAlpha(1, 1f, GameManager.Instance.RestartScene));
    }

    public void PauseSwitch()
    {
        bool gamePaused = PauseUI.activeSelf;

        if (gamePaused)
        {
            SwitchToUI(InGameUI.gameObject);
            ControlsManager.Instance.SwitchToCharacterControls();
            TimeManager.Instance.ResumeTime();
        }
        else
        {
            SwitchToUI(PauseUI);
            ControlsManager.Instance.SwitchToUIControls();
            TimeManager.Instance.PauseTime();
    
        }
    }
    public void ShowGameOverUI(string message = "GAME OVER!")
    {
        SwitchToUI(GameOverUI.gameObject);
        Time.timeScale = 0;
        GameOverUI.ShowGameOverMessage(message);
    }

    public void ShowVictoryScreenUI()
    {
        StartCoroutine(ChangeImageAlpha(1, 1.5f, SwitchToVictorySreenUI));
    }

    private void SwitchToVictorySreenUI()
    {
        SwitchToUI(VictoryScreenUI);

        Color color = fadeImage.color;
        color.a = 0;

        fadeImage.color = color;
    }

    private void AssignInputsUI()
    {
        PlayerControls controls = GameManager.Instance.Player.controls;

        controls.UI.UIPause.performed += conext => PauseSwitch();
    }

    private IEnumerator StartGameSequence()
    {
        //StartCoroutine(ChangeImageAlpha(1, 1f, null));
        //yield return new WaitForSeconds(1);

        //SwitchToUI(InGameUI.gameObject);
        //GameManager.Instance.GameStart();

        //StartCoroutine(ChangeImageAlpha(0, 1f, null));

        bool quickStart = GameManager.Instance.QuickStart;

        //THIS SHOULD BE UNCOMMENTED BEFORE MAKING A BUILD
        if (quickStart == false)
        {
            fadeImage.color = Color.black;
            StartCoroutine(ChangeImageAlpha(1, 1, null));
            yield return new WaitForSeconds(1);

        }

        yield return null;
        SwitchToUI(InGameUI.gameObject);
        AudioManager.Instance.StopAllBGM();
        GameManager.Instance.GameStart();

        if (quickStart)
            StartCoroutine(ChangeImageAlpha(0, .1f, null));
        else
            StartCoroutine(ChangeImageAlpha(0, 1f, null));

    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, Action onComplete)
    {
        float time = 0;
        Color currentColor = fadeImage.color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        onComplete?.Invoke();
    }

    [ContextMenu("Assign Audio To Buttons")]
    public void AssignAudioListenesrsToButtons()
    {
        UI_Button[] buttons = FindObjectsOfType<UI_Button>(true);

        foreach (var button in buttons)
        {
            button.AssignAudioSource();
        }
    }

}
