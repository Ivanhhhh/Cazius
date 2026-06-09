using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Define music track options
    public enum MusicTrack
    {
        MainMenu,
        Eden1,
        Purgatory1
    }

    // Pair a track enum with an AudioClip
    [System.Serializable]
    public class MusicTrackClip
    {
        public MusicTrack track;
        public AudioClip clip;
    }

    public static MusicManager Instance;

    // List to assign in inspector
    public List<MusicTrackClip> trackClips = new List<MusicTrackClip>();

    private Dictionary<MusicTrack, AudioClip> musicDict;
    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    [SerializeField] private float defaultVolume = 0.3f;
    [SerializeField] private float defaultFadeToNewTrack = 0.5f;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = 0.5f;

        // Build lookup dictionary from inspector-assigned clips
        musicDict = new Dictionary<MusicTrack, AudioClip>();
        foreach (var trackClip in trackClips)
        {
            if (trackClip != null && trackClip.clip != null)
                musicDict[trackClip.track] = trackClip.clip;
        }

        PlayTrack(MusicTrack.MainMenu);
    }

    /*
    void OnEnable()
    {
        // Subscribe to scene load events
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Switch music based on scene name
        switch (scene.name)
        {
            case "Eden 1":
                PlayTrack(MusicTrack.Eden1);
                break;
            case "Purgatory 1":
                PlayTrack(MusicTrack.Purgatory1);
                break;
            default:
                FadeOutAndStop(0.5f);
                break;
        }
    }
    */

    //N ew code to wok with new WorldChangeManager
    private void OnEnable()
    {
        StartCoroutine(WaitForWorldChangeManager());
    }

    private IEnumerator WaitForWorldChangeManager()
    {
        while (WorldChangeManager.Instance == null)
            yield return null;

        WorldChangeManager.Instance.SwapToPurgatoryEvent += PlayPurgatoryTrack;
        WorldChangeManager.Instance.SwapToEdenEvent += PlayEdenTrack;
    }

    private void OnDisable()
    {
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= PlayPurgatoryTrack;
        WorldChangeManager.Instance.SwapToEdenEvent -= PlayEdenTrack;
    }


    private void PlayPurgatoryTrack() { PlayTrack(MusicTrack.Purgatory1); }
    private void PlayEdenTrack() { PlayTrack(MusicTrack.Eden1); }


    // Play specific track by Enum
    public void PlayTrack(MusicTrack track)
    {
        if (!musicDict.TryGetValue(track, out AudioClip newClip))
        {
            Debug.LogWarning($"MusicManager: Track {track} not found!");
            return;
        }

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeToNewTrack(newClip, defaultFadeToNewTrack));
    }

    // Fade out, switch track, fade in
    private IEnumerator FadeToNewTrack(AudioClip newClip, float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(defaultVolume, 0, t / duration);
            yield return null;
        }
        audioSource.volume = 0;

        audioSource.clip = newClip;
        audioSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(0, defaultVolume, t / duration);
            yield return null;
        }
        audioSource.volume = defaultVolume;
    }

    public void FadeOutAndStop(float duration = 1f)
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            audioSource.volume = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }

        audioSource.volume = 0;
        audioSource.Stop();
    }
}
