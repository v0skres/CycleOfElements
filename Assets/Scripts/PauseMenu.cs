using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;          // панель меню паузы
    public Button continueButton;
    public Button exitToWorldMapButton;
    public GameObject hintsPanel;          // панель с подсказками по стихиям (опционально)

    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
        if (continueButton != null)
            continueButton.onClick.AddListener(Resume);
        if (exitToWorldMapButton != null)
            exitToWorldMapButton.onClick.AddListener(ExitToWorldMap);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    void Pause()
    {
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void Resume()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void ExitToWorldMap()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.BackToWorldMap();
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("WorldMap");
    }

    // Опционально: показать/скрыть панель подсказок
    public void ToggleHints()
    {
        if (hintsPanel != null)
            hintsPanel.SetActive(!hintsPanel.activeSelf);
    }
}