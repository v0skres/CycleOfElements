using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector] public int selectedZoneIndex;
    [HideInInspector] public string selectedZoneName;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame() => SceneManager.LoadScene("WorldMap");

    // НОВЫЙ МЕТОД для загрузки по имени сцены (для кнопок на карте мира)
    public void SelectZone(string sceneName)
    {
        Debug.Log($"Загрузка сцены: {sceneName}");
        SceneManager.LoadScene(sceneName);
    }

    // Старый метод (можно оставить для совместимости, если он где-то используется)
    public void SelectZone(int index, string zoneName)
    {
        selectedZoneIndex = index;
        selectedZoneName = zoneName;
        SceneManager.LoadScene("BattleScene");
    }

    public void BackToWorldMap() => SceneManager.LoadScene("WorldMap");
    public void BackToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void QuitGame() => Application.Quit();
}