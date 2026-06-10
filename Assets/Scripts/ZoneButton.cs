using UnityEngine;
using UnityEngine.UI;

public class ZoneButton : MonoBehaviour
{
    public int zoneIndex;      // 0=Огонь, 1=Вода, 2=Земля, 3=Металл, 4=Дерево
    public string sceneName;   // "Fire", "Water", "Earth", "Metal", "Wood"

    void Start()
    {
        Button btn = GetComponent<Button>();
        if (btn != null)
            btn.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.SelectZone(sceneName, zoneIndex);
        else
            Debug.LogError("GameManager.Instance не найден!");
    }
}