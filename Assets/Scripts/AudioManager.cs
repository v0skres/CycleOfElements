using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip mainMenuMusic;
    public AudioClip worldMapMusic;
    public AudioClip battleMusic;

    [Header("SFX")]
    public AudioClip buttonClick;
    public AudioClip cardPlay;
    public AudioClip enemyHit;
    public AudioClip playerHit;
    public AudioClip victory;
    public AudioClip defeat;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float musicVolume = 0.5f;
    private float sfxVolume = 0.7f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.volume = musicVolume;

            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.volume = sfxVolume;

            LoadVolumes();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForScene(scene.name);
    }

    void PlayMusicForScene(string sceneName)
    {
        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(mainMenuMusic);
                break;
            case "WorldMap":
                PlayMusic(worldMapMusic);
                break;
            default:
                PlayMusic(battleMusic);
                break;
        }
    }

    void PlayMusic(AudioClip clip)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayButtonClick() => PlaySFX(buttonClick);

    // Громкость музыки
    public void SetMusicVolume(float vol)
    {
        musicVolume = Mathf.Clamp01(vol);
        musicSource.volume = musicVolume;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public float GetMusicVolume() => musicVolume;

    // Громкость эффектов
    public void SetSFXVolume(float vol)
    {
        sfxVolume = Mathf.Clamp01(vol);
        sfxSource.volume = sfxVolume;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public float GetSFXVolume() => sfxVolume;

    private void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }
}