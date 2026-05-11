using System;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    public event Action OnPaused;
    public event Action OnResumed;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Toggle()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            OnPaused?.Invoke();
        }
        else
        {
            Time.timeScale = 1f;
            AudioListener.pause = false;
            OnResumed?.Invoke();
        }
    }
}
