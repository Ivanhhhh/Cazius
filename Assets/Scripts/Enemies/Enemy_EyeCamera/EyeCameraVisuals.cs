
using UnityEngine;

public class EyeCameraVisuals : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _beamOrigin;
    [SerializeField] private Light _spotlight;
    [SerializeField] private LineRenderer _beam;
    [SerializeField] private ParticleSystem _contactParticles;
    [SerializeField] private ParticleSystem _damageParticles;

    [Header("Spotlight")]
    [SerializeField] private float _trackingLightMultiplier = 2f;
    [SerializeField] private float _lightPulseAmount = 0.15f;
    [SerializeField] private float _damageLightBoost = 2f;

    [Header("Beam")]
    [SerializeField] private Vector3 _beamTargetOffset = new Vector3(0f, -0.5f, 0f);
    [SerializeField] private Color _beamColor = new Color(1f, 0.1f, 0.1f, 0.6f);
    [SerializeField] private float _beamWidth = 0.025f;
    [SerializeField] private float _beamPulseSpeed = 8f;
    [SerializeField] private float _beamNoiseAmount = 0.015f;
    [SerializeField] private float _beamNoiseSpeed = 12f;
    [Range(2, 32)]
    [SerializeField] private int _beamSegments = 12;

    [Header("Transitions")]
    [SerializeField] private float _fadeSpeed = 5f;
    [SerializeField] private float _damageFlashDuration = 0.15f;
    [SerializeField] private float _damageBeamMultiplier = 2.5f;

    private Transform _target;
    private Vector3 _targetOffset;

    private float _originalLightIntensity;
    private float _visibility;
    private float _damageFlash;

    private bool _isTracking;

    private void Awake()
    {
        if (_beamOrigin == null)
            _beamOrigin = transform;

        if (_spotlight != null)
            _originalLightIntensity = _spotlight.intensity;

        if (_beam != null)
        {
            _beam.useWorldSpace = true;
            _beam.positionCount = Mathf.Max(2, _beamSegments);
            _beam.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
            _beam.enabled = false;
        }

        if (_contactParticles != null)
            _contactParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (_damageParticles != null)
            _damageParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    private void LateUpdate()
    {
        if (_isTracking && _target == null)
            StopTracking();

        _visibility = Mathf.MoveTowards(
            _visibility,
            _isTracking ? 1f : 0f,
            _fadeSpeed * Time.deltaTime
        );

        _damageFlash = Mathf.MoveTowards(
            _damageFlash,
            0f,
            Time.deltaTime / Mathf.Max(0.01f, _damageFlashDuration)
        );

        UpdateSpotlight();
        UpdateBeam();

        if (_contactParticles != null && _target != null)
            _contactParticles.transform.position = GetTargetPosition();

        if (!_isTracking && _visibility <= 0f)
            _target = null;
    }

    public void StartTracking(Transform target, Vector3 offset)
    {
        if (target == null)
            return;

        _target = target;
        _targetOffset = offset;
        _isTracking = true;

        if (_contactParticles != null && !_contactParticles.isPlaying)
            _contactParticles.Play();
    }

    public void StopTracking()
    {
        _isTracking = false;

        if (_contactParticles != null)
            _contactParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    public void OnDamage()
    {
        if (!_isTracking || _target == null)
            return;

        _damageFlash = 1f;

        if (_damageParticles != null)
        {
            _damageParticles.transform.position = GetTargetPosition();
            _damageParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _damageParticles.Play();
        }
    }

    private void UpdateSpotlight()
    {
        if (_spotlight == null)
            return;

        float intensity = Mathf.Lerp(
            _originalLightIntensity,
            _originalLightIntensity * _trackingLightMultiplier,
            _visibility
        );

        float pulse = Mathf.Sin(Time.time * _beamPulseSpeed);

        intensity *= 1f + pulse * _lightPulseAmount * _visibility;
        intensity += _originalLightIntensity * _damageLightBoost * _damageFlash;

        _spotlight.intensity = Mathf.Max(0f, intensity);
    }

    private void UpdateBeam()
    {
        if (_beam == null)
            return;

        if (_target == null || _visibility <= 0.001f)
        {
            _beam.enabled = false;
            return;
        }

        Vector3 start = _beamOrigin.position;
        Vector3 end = GetTargetPosition();
        Vector3 direction = end - start;

        if (direction.sqrMagnitude < 0.0001f)
        {
            _beam.enabled = false;
            return;
        }

        _beam.enabled = true;

        float pulse = Mathf.Sin(Time.time * _beamPulseSpeed);

        float width = _beamWidth * (1f + pulse * 0.1f);
        width *= 1f + _damageFlash * (_damageBeamMultiplier - 1f);

        _beam.widthMultiplier = width;

        Color color = _beamColor;
        color.a *= _visibility * Mathf.Clamp01(0.85f + pulse * 0.15f + _damageFlash);

        _beam.startColor = color;
        _beam.endColor = color;

        Vector3 forward = direction.normalized;
        Vector3 side = Vector3.Cross(forward, Vector3.up);

        if (side.sqrMagnitude < 0.0001f)
            side = Vector3.Cross(forward, Vector3.forward);

        side.Normalize();

        Vector3 up = Vector3.Cross(forward, side).normalized;

        int segments = _beam.positionCount;

        for (int i = 0; i < segments; i++)
        {
            float t = (float)i / (segments - 1);

            Vector3 position = Vector3.Lerp(start, end, t);

            float fade = Mathf.Sin(t * Mathf.PI);


            float time = Time.time * _beamNoiseSpeed;

            float wave1 = Mathf.Sin(t * Mathf.PI * 6f - time);
            float wave2 = Mathf.Sin(t * Mathf.PI * 9f + time * 0.7f);

            float noise1 = Mathf.PerlinNoise(t * 8f, time * 0.3f) * 2f - 1f;
            float noise2 = Mathf.PerlinNoise(t * 8f + 100f, time * 0.3f) * 2f - 1f;

            Vector3 noise = (
                side * (wave1 * 0.7f + noise1 * 0.3f) +
                up * (wave2 * 0.7f + noise2 * 0.3f)
            ) * _beamNoiseAmount * fade;


            _beam.SetPosition(i, position + noise);
        }
    }

    private Vector3 GetTargetPosition()
    {
        return _target.position + _targetOffset + _beamTargetOffset;
    }

    private void OnDisable()
    {
        _isTracking = false;
        _target = null;
        _visibility = 0f;
        _damageFlash = 0f;

        if (_spotlight != null)
            _spotlight.intensity = _originalLightIntensity;

        if (_beam != null)
            _beam.enabled = false;

        if (_contactParticles != null)
            _contactParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (_damageParticles != null)
            _damageParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }
}
