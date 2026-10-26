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
    [SerializeField] private ExitConfirmationPanel exitPanel;

    private EkControllerSystem inputPause;
    private bool isPaused;

    private void Awake()
    {
        inputPause = new EkControllerSystem();
        pausePanel.SetActive(false);
        SettingsMenu.SetActive(false);
        SettingsGameplay.SetActive(false);
    }

    private void Start()
    {
        TogglePause();
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
        TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
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
        exitPanel.Show();
    }

    public void StartGame()
    {
        TogglePause();
        MainMenuPanel.SetActive(false);
    }
}