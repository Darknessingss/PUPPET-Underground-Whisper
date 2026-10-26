using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ExitConfirmationPanel : MonoBehaviour
{
    [SerializeField] private GameObject ConfirmPanel;
    [SerializeField] private GameObject MainMenuPanel;

    private void Awake()
    {
        ConfirmPanel.SetActive(false);
    }

    public void Show()
    {
        ConfirmPanel.SetActive(true);
    }

    public void Hide()
    {
        ConfirmPanel.SetActive(false);
    }

    public void OnConfirmExit()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ExitGameOver()
    {
        Application.Quit();
    }

    public void PanelExit()
    {
        MainMenuPanel.SetActive(true);
    }

    public void OnCancelExit()
    {
        Hide();
    }
}