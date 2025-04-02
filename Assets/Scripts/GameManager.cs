using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle sfxToggle; // Теперь: ON=выкл, OFF=вкл

    [Header("Scene Management")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject settingsPanel;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_ENABLED_KEY = "SFXEnabled";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Initialize()
    {
        // 1. Настройка музыки
        musicSlider.value = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);

        // 2. Настройка SFX (инвертированная логика)
        bool sfxEnabled = PlayerPrefs.HasKey(SFX_ENABLED_KEY) 
            ? PlayerPrefs.GetInt(SFX_ENABLED_KEY) == 1 
            : true; // По умолчанию звук ВКЛЮЧЕН
        
        // Инвертируем для Toggle: enabled=false → IsOn=false
        sfxToggle.SetIsOnWithoutNotify(!sfxEnabled);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);

        // 3. Применяем настройки
        ApplyAudioSettings(musicSlider.value, sfxEnabled);
    }

    private void ApplyAudioSettings(float musicVolume, bool sfxEnabled)
    {
        // Музыка
        if (MusicPlayer.Instance != null)
        {
            MusicPlayer.Instance.SetMusicVolume(musicVolume);
        }

        // SFX (инвертированная логика)
        AudioListener.volume = sfxEnabled ? 1 : 0;
    }

    public void OnMusicVolumeChanged(float value)
    {
        float volume = Mathf.Clamp(value, 0.001f, 1f);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        
        if (MusicPlayer.Instance != null)
        {
            MusicPlayer.Instance.SetMusicVolume(volume);
        }
    }

    public void OnSFXToggleChanged(bool isOn)
    {
        // isOn=true → звук ВЫКЛЮЧЕН
        bool sfxEnabled = !isOn;
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, sfxEnabled ? 1 : 0);
        AudioListener.volume = sfxEnabled ? 1 : 0;
    }

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(true);
        settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        mainMenuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene(levelIndex);
    }

    public void QuitGame()
    {
        PlayerPrefs.Save();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}