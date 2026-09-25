using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class OrbitMovement : MonoBehaviour
{
    [Header("Referencias de Órbita")]
    public Transform _centerPoint;
    public float _radius = 5f;
    public float _orbitSpeed = 2f;
    public Vector3 _orbitTilt = Vector3.zero;

    [Header("Atributos de la Bala")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _lifeTime = 5f;

    [Header("Efecto de Aparición")]
    [SerializeField] private float _mergeSpeed = 15f;

    // =========================================================
    // FASE DE CARGA
    // =========================================================
    [Header("Pre-Fire Charge")]
    [Tooltip("Cuánto tarda el shader en ir de startValue a endValue. Cuando llega a endValue, se dispara.")]
    [SerializeField] private float _shaderChangeDuration = 1.5f;

    [Header("Shader (Material Property Block)")]
    [SerializeField] private string _sliderPropertyName = "_ChangeSlider";
    [SerializeField] private float _sliderStartValue = 1f;
    [SerializeField] private float _sliderEndValue = 0f;
    [SerializeField] private Renderer _renderer;

    [Header("VFX de Carga")]
    [SerializeField] private GameObject _chargeVFX;

    // --- Internos ---
    private float _currentAngle;
    private OrbitManager _manager;
    private Rigidbody _rb;

    private bool _isBullet = false;
    private bool _isMerging = true;
    private bool _isCharging = false;

    private MaterialPropertyBlock _propBlock;
    private int _sliderID;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        _rb.isKinematic = true;

        _sliderID = Shader.PropertyToID(_sliderPropertyName);
        _propBlock = new MaterialPropertyBlock();
    }

    // =========================================================
    // INICIALIZACIÓN (al crear o sacar del pool)
    // =========================================================
    public void InitializeOrbit(float startingAngle, OrbitManager manager)
    {
        _currentAngle = startingAngle;
        _manager = manager;
        _isBullet = false;
        _isMerging = true;
        _isCharging = false;
        _rb.isKinematic = true;

        // Shader al valor inicial (1)
        SetSliderValue(_sliderStartValue);

        // VFX detenido y reseteado (no emite nada)
        if (_chargeVFX != null)
        {
            _chargeVFX.SetActive(false);
        }
    }

    void Update()
    {
        if (_isBullet || _centerPoint == null) return;
        if (_isCharging) return;

        _currentAngle += _orbitSpeed * Time.deltaTime;
        Vector3 localOrbitPosition = new Vector3(Mathf.Cos(_currentAngle) * _radius, 0f, Mathf.Sin(_currentAngle) * _radius);
        Vector3 tiltedOrbitPosition = Quaternion.Euler(_orbitTilt) * localOrbitPosition;
        Vector3 worldOffset = _centerPoint.rotation * tiltedOrbitPosition;
        Vector3 idealPosition = _centerPoint.position + worldOffset;

        if (_isMerging)
        {
            float orbitLinearSpeed = Mathf.Abs(_orbitSpeed * _radius);
            float safeMergeSpeed = Mathf.Max(_mergeSpeed, orbitLinearSpeed + 5f);

            transform.position = Vector3.MoveTowards(transform.position, idealPosition, safeMergeSpeed * Time.deltaTime);

            if (transform.position == idealPosition)
                _isMerging = false;
        }
        else
        {
            transform.position = idealPosition;
        }

        Vector3 directionToCenter = (_centerPoint.position - transform.position).normalized;
        if (directionToCenter != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(directionToCenter);
    }

    // =========================================================
    // FIRE: activa VFX + anima shader + dispara al terminar
    // =========================================================
    public void FireAsBullet(Vector3 targetPosition, float bulletSpeed)
    {
        if (_isBullet || _isCharging) return;
        StartCoroutine(ChargeAndFireRoutine(targetPosition, bulletSpeed));
    }

    private IEnumerator ChargeAndFireRoutine(Vector3 targetPosition, float bulletSpeed)
    {
        _isCharging = true;

        // 1. Arrancar VFX
        if (_chargeVFX != null)
            _chargeVFX.SetActive(true);

        // 2. Animar el shader de startValue a endValue
        float elapsed = 0f;
        while (elapsed < _shaderChangeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _shaderChangeDuration);
            float value = Mathf.Lerp(_sliderStartValue, _sliderEndValue, t);
            SetSliderValue(value);
            yield return null;
        }
        SetSliderValue(_sliderEndValue);

        _isCharging = false;

        ExecuteFire(targetPosition, bulletSpeed);
    }

    private void ExecuteFire(Vector3 targetPosition, float bulletSpeed)
    {
        if (_isBullet) return;

        _isBullet = true;
        _isMerging = false;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (_manager != null)
            _manager.RemoveFromOrbit(this);

        Vector3 exactDirection = (targetPosition - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(exactDirection);
        _rb.linearVelocity = exactDirection * bulletSpeed;

        Destroy(gameObject, _lifeTime);
    }

    // =========================================================
    // MATERIAL PROPERTY BLOCK
    // =========================================================
    private void SetSliderValue(float value)
    {
        if (_renderer == null) return;

        _renderer.GetPropertyBlock(_propBlock);
        _propBlock.SetFloat(_sliderID, value);
        _renderer.SetPropertyBlock(_propBlock);
    }
}
