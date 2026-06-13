using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private const int TOTAL_ZONES = 6; // 0-Огонь,1-Вода,2-Земля,3-Металл,4-Дерево,5-Финальная
    private bool[] zoneUnlocked;
    public int currentZoneIndex;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }

        PlayerPrefs.DeleteAll(); // стирает все сохранённые данные
        LoadProgress(); // загрузит настройки по умолчанию (только огонь открыт)
    }

    public void SelectZone(string sceneName, int zoneIndex)
    {
        currentZoneIndex = zoneIndex;
        SceneManager.LoadScene(sceneName);
    }

    public void UnlockNextZone(int completedZoneIndex)
    {
        int nextIndex = completedZoneIndex + 1;
        if (nextIndex < TOTAL_ZONES)
        {
            zoneUnlocked[nextIndex] = true;
            SaveProgress();
            Debug.Log($"Разблокирована зона {nextIndex}");
        }
    }

    public bool IsZoneUnlocked(int zoneIndex)
    {
        return zoneIndex >= 0 && zoneIndex < TOTAL_ZONES && zoneUnlocked[zoneIndex];
    }

    public int GetZonesCount() => TOTAL_ZONES;

    public void BackToWorldMap() => SceneManager.LoadScene("WorldMap");
    public void BackToMainMenu() => SceneManager.LoadScene("MainMenu");
    public void QuitGame() => Application.Quit();

    private void SaveProgress()
    {
        for (int i = 0; i < TOTAL_ZONES; i++)
            PlayerPrefs.SetInt($"ZoneUnlocked_{i}", zoneUnlocked[i] ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        zoneUnlocked = new bool[TOTAL_ZONES];
        for (int i = 0; i < TOTAL_ZONES; i++)
        {
            int defaultValue = (i == 0) ? 1 : 0;
            zoneUnlocked[i] = PlayerPrefs.GetInt($"ZoneUnlocked_{i}", defaultValue) == 1;
        }
    }
}