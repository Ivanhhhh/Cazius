using System;
using UnityEngine;
//using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class BossFightManager : MonoBehaviour
{
    public static BossFightManager Instance { get; private set; }

    public event Action SwapPhase;
    public event Action Die;

    [SerializeField] private float _transitionDuration = 5f;

    public float TransitionDuration => _transitionDuration;

    private bool _isDead = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
/*
    private void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            TriggerSwapPhase();

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            TriggerDie();
    }
*/

    public void TriggerSwapPhase()
    {
        if (_isDead) return;

        SwapPhase?.Invoke();
        CameraShakeManager.Instance.CameraShake(0.15f, _transitionDuration);
    }

    public void TriggerDie()
    {
        if (_isDead) return;

        _isDead = true;

        Die?.Invoke();
        CameraShakeManager.Instance.CameraShake(0.15f, _transitionDuration);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}