using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemySpawnEffect : MonoBehaviour
{
    private static readonly int AlphaClipMultID = Shader.PropertyToID("_AlphaClipMult");

    private static readonly int EffectAmountID = Shader.PropertyToID("_EffectAmount");

    [Header("SpawnOnFloor")]

    [SerializeField] private bool _willSpawn;

    [Header("Floor")]
    [SerializeField] private List<MeshRenderer> _floorRenderers = new();

    [SerializeField] private float _floorFadeInDuration = 0.5f;
    [SerializeField] private float _floorFadeOutDuration = 1f;


    [Header("Blood VFX")]
    [SerializeField] private List<VisualEffect> _bloodVFX = new();


    [Header("Enemy")]
    [SerializeField] private List<SkinnedMeshRenderer> _enemyRenderers = new();

    [SerializeField] private float _enemyEffectStart = 1f;
    [SerializeField] private float _enemyEffectEnd = 0f;
    [SerializeField] private float _bloodRemovalStart = 0.4f;
    [SerializeField] private float _bloodRemovalDuration = 2.5f;


    [Header("Enemy Movement")]
    [SerializeField] private Transform _enemyModel;

    [SerializeField] private Transform _startPosition;
    [SerializeField] private Transform _endPosition;

    [SerializeField] private float _riseDuration = 2f;

    [SerializeField]
    private AnimationCurve _riseCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


    [Header("Timing")]
    [SerializeField] private float _delayBeforeRise = 0.3f;
    [SerializeField] private float _delayBeforeCleanup = 0.5f;


    private Coroutine _spawnCoroutine;

    private MaterialPropertyBlock _propertyBlock;


    private void Awake()
    {
        _propertyBlock = new MaterialPropertyBlock();

        SetFloorAlpha(0f);

        SetEnemyEffect(_enemyEffectStart);

        DisableBloodVFX();

        if (_willSpawn)
        {
        PrepareEffect();
        }
    }


    public void PrepareEffect()
    {
        SetFloorAlpha(0f);

        SetEnemyEffect(_enemyEffectStart);

        if (_enemyModel != null && _startPosition != null)
            _enemyModel.position = _startPosition.position;

        DisableBloodVFX();
    }


    public void PlaySpawnEffect()
    {
        if (_spawnCoroutine != null)
            StopCoroutine(_spawnCoroutine);

        _spawnCoroutine = StartCoroutine(SpawnRoutine());
    }


    private IEnumerator SpawnRoutine()
    {

        SetFloorAlpha(0f);
        SetEnemyEffect(_enemyEffectStart);

        if (_enemyModel != null && _startPosition != null)
            _enemyModel.position = _startPosition.position;


        EnableBloodVFX();


        StartCoroutine(
            LerpFloorAlpha(
                0f,
                1f,
                _floorFadeInDuration
            )
        );

        yield return new WaitForSeconds(_delayBeforeRise);


        Vector3 startPos = _startPosition.position;
        Vector3 endPos = _endPosition.position;

        float timer = 0f;
        float bloodTimer = 0f;

        bool bloodRemovalStarted = false;

        while (timer < _riseDuration)
        {
            timer += Time.deltaTime;

            float normalizedTime =
                Mathf.Clamp01(timer / _riseDuration);

            float movementTime =
                _riseCurve.Evaluate(normalizedTime);


            _enemyModel.position = Vector3.Lerp(
                startPos,
                endPos,
                movementTime
            );


            if (normalizedTime >= _bloodRemovalStart)
            {
                bloodRemovalStarted = true;

                bloodTimer += Time.deltaTime;

                float bloodT = Mathf.Clamp01(
                    bloodTimer / _bloodRemovalDuration
                );

                float effectAmount = Mathf.Lerp(
                    _enemyEffectStart,
                    _enemyEffectEnd,
                    bloodT
                );

                SetEnemyEffect(effectAmount);
            }


            yield return null;
        }


        _enemyModel.position = endPos;


        if (bloodRemovalStarted)
        {
            while (bloodTimer < _bloodRemovalDuration)
            {
                bloodTimer += Time.deltaTime;

                float bloodT = Mathf.Clamp01(
                    bloodTimer / _bloodRemovalDuration
                );

                float effectAmount = Mathf.Lerp(
                    _enemyEffectStart,
                    _enemyEffectEnd,
                    bloodT
                );

                SetEnemyEffect(effectAmount);

                yield return null;
            }
        }

        SetEnemyEffect(_enemyEffectEnd);


        yield return new WaitForSeconds(_delayBeforeCleanup);


        StopBloodVFX();


        yield return StartCoroutine(
            LerpFloorAlpha(
                1f,
                0f,
                _floorFadeOutDuration
            )
        );


        DisableBloodVFX();

        _spawnCoroutine = null;
    }


    private IEnumerator LerpFloorAlpha(
        float startValue,
        float endValue,
        float duration)
    {
        if (duration <= 0f)
        {
            SetFloorAlpha(endValue);
            yield break;
        }

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t =
                Mathf.Clamp01(timer / duration);

            float value =
                Mathf.Lerp(startValue, endValue, t);

            SetFloorAlpha(value);

            yield return null;
        }

        SetFloorAlpha(endValue);
    }

    private void SetFloorAlpha(float value)
    {
        foreach (MeshRenderer renderer in _floorRenderers)
        {
            if (renderer == null)
                continue;

            renderer.GetPropertyBlock(_propertyBlock);

            _propertyBlock.SetFloat(
                AlphaClipMultID,
                value
            );

            renderer.SetPropertyBlock(_propertyBlock);

            _propertyBlock.Clear();
        }
    }

    private void SetEnemyEffect(float value)
    {
        foreach (SkinnedMeshRenderer renderer in _enemyRenderers)
        {
            if (renderer == null)
                continue;

            renderer.GetPropertyBlock(_propertyBlock);

            _propertyBlock.SetFloat(
                EffectAmountID,
                value
            );

            renderer.SetPropertyBlock(_propertyBlock);

            _propertyBlock.Clear();
        }
    }

    private void EnableBloodVFX()
    {
        foreach (VisualEffect vfx in _bloodVFX)
        {
            if (vfx == null)
                continue;

            vfx.gameObject.SetActive(true);

            vfx.Reinit();
            vfx.Play();
        }
    }


    private void StopBloodVFX()
    {
        foreach (VisualEffect vfx in _bloodVFX)
        {
            if (vfx == null)
                continue;

            vfx.Stop();
        }
    }


    private void DisableBloodVFX()
    {
        foreach (VisualEffect vfx in _bloodVFX)
        {
            if (vfx == null)
                continue;

            vfx.Stop();
            vfx.gameObject.SetActive(false);
        }
    }
}