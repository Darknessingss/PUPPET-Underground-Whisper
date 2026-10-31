using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ManagerInterface : MonoBehaviour
{
    [Header("Панели")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject MainMenuPanel;

    [SerializeField] private GameObject SettingsMenu;
    [SerializeField] private GameObject SettingsGameplay;
    [SerializeField] private GameObject exitPanel;
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private GameObject MindMapPanel;

    private EkControllerSystem inputPause;
    private bool isPaused;

    private void Awake()
    {
        inputPause = new EkControllerSystem();
        pausePanel.SetActive(false);
        SettingsMenu.SetActive(false);
        SettingsGameplay.SetActive(false);
        deathPanel.SetActive(false);
        MindMapPanel.SetActive(false);
    }

    private void Start()
    {
        isPaused = true;
        Time.timeScale = 0f;
        MainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
    }
    private void OnEnable()
    {
        inputPause.Enable();
        inputPause.EscapeSystem.ExitGameplay.Enable();
        inputPause.EscapeSystem.ExitGameplay.performed += OnEscape;
    }

    private void OnDisable()
    {
        inputPause.EscapeSystem.ExitGameplay.performed -= OnEscape;
        inputPause.EscapeSystem.ExitGameplay.Disable();
        inputPause.Disable();
    }

    private void OnDestroy()
    {
        inputPause?.Dispose();
    }

    private void OnEscape(InputAction.CallbackContext context)
    {
        if (MainMenuPanel.activeSelf || SettingsMenu.activeSelf
            || SettingsGameplay.activeSelf || pausePanel.activeSelf || deathPanel.activeSelf || exitPanel.activeSelf)
            return;

        TogglePause(!isPaused);
    }

    public void TogglePause(bool paused)
    {
        isPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
        pausePanel.SetActive(paused && !MainMenuPanel.activeSelf
                                   && !SettingsMenu.activeSelf
                                   && !SettingsGameplay.activeSelf
                                   && !deathPanel.activeSelf);
    }

    public void Resume()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettingsMenu()
    {
        SettingsMenu.SetActive(true);
        pausePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }
    public void OpenSettingsGamePlay()
    {
        SettingsGameplay.SetActive(true);
        pausePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
    }

    public void CloseSettingsGameplay()
    {
        SettingsGameplay.SetActive(false);
        MainMenuPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void CloseSettingsMenu()
    {
        SettingsMenu.SetActive(false);
        MainMenuPanel.SetActive(true);
        pausePanel.SetActive(false);
    }

    public void OpenExitPanel()
    {
        pausePanel.SetActive(false);
        exitPanel.SetActive(true);
    }

    public void StartGame()
    {
        TogglePause(!isPaused);
        MainMenuPanel.SetActive(false);
    }
    public void ShowDeathScreen()
    {
        isPaused = true;
        Time.timeScale = 0f;

        pausePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        SettingsMenu.SetActive(false);
        SettingsGameplay.SetActive(false);
        exitPanel.SetActive(false);

        deathPanel.SetActive(true);
    }

    public void ShowMindMap()
    {
        TogglePause(!isPaused);

        MindMapPanel.SetActive(true);

        pausePanel.SetActive(false);
        MainMenuPanel.SetActive(false);
        SettingsMenu.SetActive(false);
        SettingsGameplay.SetActive(false);
        exitPanel.SetActive(false);
        deathPanel.SetActive(false);
    }

    public void CloseShowMap()
    {
        TogglePause(!isPaused);
        MindMapPanel.SetActive(false);
    }    
}