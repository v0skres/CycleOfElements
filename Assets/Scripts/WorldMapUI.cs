using UnityEngine;
using UnityEngine.UI;

public class WorldMapUI : MonoBehaviour
{
    public Button[] zoneButtons;

    void OnEnable()
    {
        UpdateButtons();
    }

    void UpdateButtons()
    {
        if (GameManager.Instance == null) return;
        int zonesCount = GameManager.Instance.GetZonesCount();
        for (int i = 0; i < zoneButtons.Length; i++)
        {
            if (zoneButtons[i] != null)
            {
                if (i < zonesCount)
                    zoneButtons[i].interactable = GameManager.Instance.IsZoneUnlocked(i);
                else
                    zoneButtons[i].interactable = false;
            }
        }
    }
}