using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    public GameObject settingsPanel;

    void Start()
    {
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();

        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void OnMusicVolumeChanged(float val) => AudioManager.Instance.SetMusicVolume(val);
    void OnSFXVolumeChanged(float val) => AudioManager.Instance.SetSFXVolume(val);

    public void ShowSettings() => settingsPanel.SetActive(true);
    public void HideSettings() => settingsPanel.SetActive(false);
}