using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
   [Header("UI References")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Toggle sfxToggle;
    
    [Header("Scene Management")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private Animator uiAnimator;

    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_ENABLED_KEY = "SFXEnabled";

    private void Start()
    {
        InitializeSettings();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);
        musicSlider.value = savedVolume;
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        
        bool sfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1 && savedVolume > 0;
        sfxToggle.SetIsOnWithoutNotify(!sfxEnabled);
        sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
        
        ApplyAudioSettings(savedVolume, sfxEnabled);
    }

    private void ApplyAudioSettings(float musicVolume, bool sfxEnabled)
    {
        bool soundEnabled = sfxEnabled && musicVolume > 0;
        AudioListener.volume = soundEnabled ? 1 : 0;
        
        MusicPlayer musicPlayer = FindObjectOfType<MusicPlayer>();
        if (musicPlayer != null)
        {
            musicPlayer.SetMusicVolume(soundEnabled ? musicVolume : 0);
        }
    }

    public void OnMusicVolumeChanged(float value)
    {
        float volume = Mathf.Clamp(value, 0f, 1f);
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        
        if (Mathf.Approximately(volume, 0f))
        {
            sfxToggle.SetIsOnWithoutNotify(true);
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, 0);
        }
        
        ApplyAudioSettings(volume, PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1);
    }

    public void OnSFXToggleChanged(bool isOn)
    {
        bool sfxEnabled = !isOn;
        PlayerPrefs.SetInt(SFX_ENABLED_KEY, sfxEnabled ? 1 : 0);
        ApplyAudioSettings(PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f), sfxEnabled);
    }

    public void ShowMainMenu()
    {
        uiAnimator.SetBool("isSettingsOpened", false);
        uiAnimator.SetBool("isLevelChooseOpened", false);
        // mainMenuPanel.SetActive(true);
        // levelSelectPanel.SetActive(false);
        // settingsPanel.SetActive(false);
    }

    public void ShowLevelSelect()
    {
        uiAnimator.SetBool("isLevelChooseOpened", true);
        // mainMenuPanel.SetActive(false);
        // levelSelectPanel.SetActive(true);
        // settingsPanel.SetActive(false);
    }

    public void ShowSettings()
    {
        uiAnimator.SetBool("isSettingsOpened", true);
        // mainMenuPanel.SetActive(false);
        // levelSelectPanel.SetActive(false);
        // settingsPanel.SetActive(true);
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