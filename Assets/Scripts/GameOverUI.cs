using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;

    void Start()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Time.timeScale = 0f; // Приостанавливаем игру
    }

    public void RestartBattle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToWorldMap()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
            GameManager.Instance.BackToWorldMap();
        else
            SceneManager.LoadScene("WorldMap");
    }
}