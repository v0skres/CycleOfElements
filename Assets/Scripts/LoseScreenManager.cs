using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseScreenManager : MonoBehaviour
{
    public void RestartBattle()
    {
        // Перезагружаем текущую сцену (бой заново)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToWorldMap()
    {
        // Загружаем карту мира
        SceneManager.LoadScene("WorldMap");
    }
}