using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCounterVisuals : MonoBehaviour
{
    [System.Serializable]
    private class EffectSettings
    {
        [Header("Spawn")]
        public Transform spawnPoint;

        [Min(0f)]
        public float startDelay;

        public List<GameObject> effectPrefabs = new List<GameObject>();

        [Min(0f)]
        public float effectLifetime = 1f;

        public bool useActionFrame = true;

        [Range(0.01f, 1f)]
        public float actionFrameTimeScale = 0.15f;

        [Min(0f)]
        public float actionFrameDuration = 0.04f;

        public DamageFeedback damageFeedback;
        public CameraShake cameraShake;
    }

    [Header("Parry")]
    [SerializeField]
    private EffectSettings _parryEffects =
        new EffectSettings();

    [Header("Counterattack")]
    [SerializeField]
    private EffectSettings _counterattackEffects =
        new EffectSettings();

    private Coroutine _actionFrameCoroutine;

    private float _savedTimeScale;
    private float _savedFixedDeltaTime;
    private bool _actionFrameActive;

    public void PlayParryVisuals()
    {
        StartCoroutine(PlayEffectRoutine(_parryEffects));
    }

    public void PlayCounterattackVisuals()
    {
        StartCoroutine(PlayEffectRoutine(_counterattackEffects));
    }

    private IEnumerator PlayEffectRoutine(EffectSettings settings)
    {
        if (settings.startDelay > 0f)
        {
            yield return new WaitForSecondsRealtime(settings.startDelay);
        }

        if (settings.spawnPoint == null)
        {
            Debug.LogWarning(
                $"{name}: No spawn point assigned for this combat effect.",
                this
            );

            yield break;
        }

        List<GameObject> spawnedEffects = new List<GameObject>();

        foreach (GameObject effectPrefab in settings.effectPrefabs)
        {
            if (effectPrefab == null)
                continue;

            GameObject effectInstance = Instantiate(
                effectPrefab,
                settings.spawnPoint.position,
                settings.spawnPoint.rotation
            );

            effectInstance.SetActive(true);

            RestartParticleSystems(effectInstance);

            spawnedEffects.Add(effectInstance);
        }

        if (settings.useActionFrame &&
            settings.actionFrameDuration > 0f)
        {
            StartActionFrame(
                settings.actionFrameTimeScale,
                settings.actionFrameDuration
            );
        }

        _parryEffects.damageFeedback.ParryAttack();
        _parryEffects.cameraShake.DamageShake();

        if (settings.effectLifetime > 0f)
        {
            yield return new WaitForSecondsRealtime(
                settings.effectLifetime
            );
        }

        foreach (GameObject spawnedEffect in spawnedEffects)
        {
            if (spawnedEffect != null)
                Destroy(spawnedEffect);
        }
    }

    private void RestartParticleSystems(GameObject effectObject)
    {
        ParticleSystem[] particleSystems =
            effectObject.GetComponentsInChildren<ParticleSystem>(true);

        foreach (ParticleSystem particleSystem in particleSystems)
        {
            particleSystem.gameObject.SetActive(true);

            particleSystem.Stop(
                true,
                ParticleSystemStopBehavior.StopEmittingAndClear
            );

            particleSystem.Play(true);
        }
    }

    private void StartActionFrame(float targetTimeScale, float duration)
    {
        if (Time.timeScale <= 0f)
            return;

        if (_actionFrameCoroutine != null)
        {
            StopCoroutine(_actionFrameCoroutine);
            RestoreTimeScale();
        }

        _actionFrameCoroutine = StartCoroutine(
            ActionFrameRoutine(targetTimeScale, duration)
        );
    }

    private IEnumerator ActionFrameRoutine(
        float targetTimeScale,
        float duration
    )
    {
        _savedTimeScale = Time.timeScale;
        _savedFixedDeltaTime = Time.fixedDeltaTime;
        _actionFrameActive = true;

        float clampedTimeScale = Mathf.Clamp(
            targetTimeScale,
            0.01f,
            1f
        );

        Time.timeScale = clampedTimeScale;

        Time.fixedDeltaTime =
            _savedFixedDeltaTime *
            (clampedTimeScale / _savedTimeScale);

        yield return new WaitForSecondsRealtime(duration);

        RestoreTimeScale();
        _actionFrameCoroutine = null;
    }

    private void RestoreTimeScale()
    {
        if (!_actionFrameActive)
            return;

        Time.timeScale = _savedTimeScale;
        Time.fixedDeltaTime = _savedFixedDeltaTime;

        _actionFrameActive = false;
    }

    private void OnDisable()
    {
        if (_actionFrameCoroutine != null)
        {
            StopCoroutine(_actionFrameCoroutine);
            _actionFrameCoroutine = null;
        }

        RestoreTimeScale();
    }
}