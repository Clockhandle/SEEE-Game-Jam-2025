using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BgmManager : MonoBehaviour
{
    public static BgmManager Instance { get; private set; }

    [Header("Music Configuration")]
    [SerializeField] private SimpleMusicData musicData;

    [Header("Volume Control")]
    [Range(0f, 1f)]
    [SerializeField] private float masterVolume = 1f;

    private AudioSource audioSource;
    private bool isPlayingMenuMusic = false;
    private bool isPlayingLevelMusic = false;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
        // Singleton - persist across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Set up AudioSource
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            audioSource.loop = true;
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Start music for current scene
        HandleMusicForCurrentScene();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleMusicForScene(scene.name);
    }

    private void HandleMusicForCurrentScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        HandleMusicForScene(currentSceneName);
    }

    private void HandleMusicForScene(string sceneName)
    {
        if (IsMenuScene(sceneName))
        {
            PlayMenuMusic();
        }
        else if (IsLevelScene(sceneName))
        {
            PlayLevelMusic();
        }
    }

    private bool IsMenuScene(string sceneName)
    {
        return sceneName.ToLower() == "mainmenu2d" ||
               sceneName.ToLower() == "mainmenu" ||
               sceneName.ToLower() == "levelselect";
    }

    private bool IsLevelScene(string sceneName)
    {
        return sceneName.StartsWith("Demo_");
    }

    private void PlayMenuMusic()
    {
        // Only switch if we're not already playing menu music
        if (!isPlayingMenuMusic && musicData != null && musicData.menuMusic != null)
        {
            SwitchToMusic(musicData.menuMusic, musicData.menuMusicVolume);
            isPlayingMenuMusic = true;
            isPlayingLevelMusic = false;
            Debug.Log("Playing Menu Music");
        }
    }

    private void PlayLevelMusic()
    {
        // Only switch if we're not already playing level music
        if (!isPlayingLevelMusic && musicData != null && musicData.levelMusic != null)
        {
            SwitchToMusic(musicData.levelMusic, musicData.levelMusicVolume);
            isPlayingLevelMusic = true;
            isPlayingMenuMusic = false;
            Debug.Log("Playing Level Music");
        }
    }

    private void SwitchToMusic(AudioClip newClip, float trackVolume)
    {
        if (audioSource.clip == newClip && audioSource.isPlaying)
        {
            // Already playing the right music, don't restart
            return;
        }

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeToNewMusic(newClip, trackVolume));
    }

    private IEnumerator FadeToNewMusic(AudioClip newClip, float trackVolume)
    {
        float fadeDuration = musicData?.fadeDuration ?? 1f;

        // Fade out current music
        if (audioSource.isPlaying && fadeDuration > 0)
        {
            float startVolume = audioSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeDuration);
                yield return null;
            }
        }

        // Switch to new music
        audioSource.clip = newClip;
        audioSource.Play();

        // Fade in new music
        if (fadeDuration > 0)
        {
            float targetVolume = trackVolume * masterVolume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / fadeDuration);
                yield return null;
            }

            audioSource.volume = targetVolume;
        }
        else
        {
            audioSource.volume = trackVolume * masterVolume;
        }

        fadeCoroutine = null;
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);

        // Update current volume
        if (isPlayingMenuMusic && musicData != null)
        {
            audioSource.volume = musicData.menuMusicVolume * masterVolume;
        }
        else if (isPlayingLevelMusic && musicData != null)
        {
            audioSource.volume = musicData.levelMusicVolume * masterVolume;
        }
    }

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public void PauseMusic()
    {
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        audioSource.UnPause();
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    public string GetCurrentMusicType()
    {
        if (isPlayingMenuMusic) return "Menu Music";
        if (isPlayingLevelMusic) return "Level Music";
        return "None";
    }
}
