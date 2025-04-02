using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }

    [Header("Music Settings")]
    [SerializeField] private AudioClip[] musicTracks;
    [SerializeField] [Range(0f, 1f)] private float defaultVolume = 0.75f;
    
    private AudioSource audioSource;
    private float _currentVolume;

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
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        
        // Загружаем сохраненную громкость
        _currentVolume = PlayerPrefs.GetFloat("MusicVolume", defaultVolume);
        audioSource.volume = _currentVolume;
        
        PlayRandomTrack();
    }

    public void SetMusicVolume(float volume)
    {
        _currentVolume = Mathf.Clamp01(volume);
        audioSource.volume = _currentVolume;
        PlayerPrefs.SetFloat("MusicVolume", _currentVolume);
    }

    private void PlayRandomTrack()
    {
        if (musicTracks.Length > 0)
        {
            audioSource.clip = musicTracks[Random.Range(0, musicTracks.Length)];
            audioSource.Play();
        }
    }
}