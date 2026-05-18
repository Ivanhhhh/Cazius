using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;
using TMPro;

public class Player_AimAndShoot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private Player_CameraRecoil _recoil;
    [SerializeField] private Animator _playerAnimator;

    [Header("Shoot")]
    [SerializeField] private float _maxDistance;
    [SerializeField] private float _shootDamageAmount;
    [SerializeField] private float _shootInterval;
    [SerializeField] private float _rayVisibleDuration;
    [SerializeField] private ParticleSystem _hitParticle;
    [SerializeField] private VisualEffect _shootVFX;
    [SerializeField] private VisualEffect _shootMuzzleVFX;
    [SerializeField] private LineRenderer _shootView;

    [Header("Ammo")]
    [SerializeField] private int _maxBullets;
    [Header("Spread")]
    [SerializeField] private float _minSpread;
    [SerializeField] private float _maxSpread;
    [SerializeField] private float _spreadPerShot;
    [SerializeField] private float _maxSpreadClamp;
    [SerializeField] private float _spreadIncreaseSpeed;
    [SerializeField] private float _spreadDecreaseSpeed;

    [Header("Crosshair")]
    [SerializeField] private Image _crossHair;
    [SerializeField] private RectTransform _crosshairTop;
    [SerializeField] private RectTransform _crosshairBottom;
    [SerializeField] private RectTransform _crosshairLeft;
    [SerializeField] private RectTransform _crosshairRight;
    [SerializeField] private float _crosshairBaseOffset;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _remainingBulletsUI;
    [SerializeField] private TextMeshProUGUI _maxBulletsUI;
    [SerializeField] private TextMeshProUGUI _pressR;

    [Header("Light Flash Settings")]
    [SerializeField] private Light flashLight;
    [SerializeField] private float flashIntensity = 8f;
    [SerializeField] private float flashRange = 8f;
    [SerializeField] private float flashDuration = 0.04f;
    private Coroutine flashCoroutine;

    [Header("Hit VFX")]
    [SerializeField] private VisualEffect _defaultHitVFX;
    [SerializeField] private float _hitVFXDestroyDelay = 2f;

    private int _remainingBullets;
    private float _shootTimer;
    public float _currentSpread;

    public bool _hasBullets => _remainingBullets > 0;

    void Start()
    {
        _shootTimer = _shootInterval;
        _remainingBullets = _maxBullets;

        _crossHair.enabled = false;
        _shootView.enabled = false;
        _crosshairTop.gameObject.SetActive(false);
        _crosshairBottom.gameObject.SetActive(false);
        _crosshairLeft.gameObject.SetActive(false);
        _crosshairRight.gameObject.SetActive(false);

        _movement._controls.Player.Recharge.started += Recharge;
        Inventory.Instance.onInventoryChanged.AddListener(UpdateUI);
        UpdateUI();

        if (flashLight == null)
            flashLight = GetComponent<Light>();

        flashLight.intensity = 0f;
        flashLight.range = flashRange;
    }
    void OnDestroy()
    {
        if (Inventory.Instance != null)
        {
            Inventory.Instance.onInventoryChanged.RemoveListener(UpdateUI);
        }
    }
    void FixedUpdate()
    {
        _shootTimer -= Time.deltaTime;
        UpdateCrosshairIndicators();
        HandleAimInput();
    }

    void HandleAimInput()
    {
        // Se suscribe y desuscribe cada frame para evitar disparos fuera del modo apuntado
        if (_movement._controls.Player.Aim.IsPressed())
            _movement._controls.Player.Shoot.started += OnShootStarted;
        else
            _movement._controls.Player.Shoot.started -= OnShootStarted;
    }

    void UpdateCrosshairIndicators()
    {
        bool isAiming = _movement._controls.Player.Aim.IsPressed();

        // El spread se actualiza siempre aunque no se esté apuntando
        // para que al volver a apuntar ya refleje la velocidad actual
        UpdateSpread();

        _crosshairTop.gameObject.SetActive(isAiming);
        _crosshairBottom.gameObject.SetActive(isAiming);
        _crosshairLeft.gameObject.SetActive(isAiming);
        _crosshairRight.gameObject.SetActive(isAiming);

        // El punto central solo aparece cuando el spread es mínimo
        _crossHair.enabled = isAiming && _currentSpread < 0.1f;

        if (!isAiming) return;

        float offset = _crosshairBaseOffset + _currentSpread * 30f;
        _crosshairTop.anchoredPosition = new Vector2(0, offset);
        _crosshairBottom.anchoredPosition = new Vector2(0, -offset);
        _crosshairLeft.anchoredPosition = new Vector2(-offset, 0);
        _crosshairRight.anchoredPosition = new Vector2(offset, 0);
    }

    void UpdateSpread()
    {
        // El 1.2f compensa que la física nunca llega exactamente a _moveSpeed
        float speedRatio = Mathf.Clamp01(_movement._currentSpeed / _movement._moveSpeed * 1.2f);
        float targetSpread = Mathf.Lerp(0f, _maxSpread, speedRatio);

        // Velocidad distinta según si el spread está subiendo o bajando
        float lerpSpeed = _currentSpread > targetSpread ? _spreadDecreaseSpeed : _spreadIncreaseSpeed;
        _currentSpread = Mathf.MoveTowards(_currentSpread, targetSpread, lerpSpeed * Time.deltaTime);

        // Evita que quede en valores microscópicos por el MoveTowards
        if (_currentSpread < 0.05f) _currentSpread = 0f;
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (_shootTimer > 0f || !_hasBullets) return;

        _recoil.OnRecoil?.Invoke();
        ManageShoot();
        _playerAnimator.SetTrigger("Shoot");
        _shootVFX.Play();
        Flash();
        _shootMuzzleVFX.SendEvent("OnPlay");
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.PlayerShootingSFX, transform.position);

        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint = Physics.Raycast(cameraRay, out RaycastHit hit, _maxDistance)
            ? hit.point
            : cameraRay.origin + cameraRay.direction * _maxDistance;

        // El spread se aplica ANTES de sumarlo para que el primer disparo sea preciso
        Vector3 direction = (targetPoint - transform.position).normalized;
        direction = Quaternion.Euler(
            UnityEngine.Random.Range(-_currentSpread, _currentSpread),
            UnityEngine.Random.Range(-_currentSpread, _currentSpread),
            0
        ) * direction;

        _currentSpread = Mathf.Clamp(_currentSpread + _spreadPerShot, 0, _maxSpreadClamp);

        if (Physics.Raycast(transform.position, direction, out RaycastHit weaponHit, _maxDistance))
        {
            HandleHit(weaponHit);
            /*_shootView.SetPosition(0, transform.position);
            _shootView.SetPosition(1, weaponHit.point);*/
        }
        else
        {
            /* _shootView.SetPosition(0, transform.position);
             _shootView.SetPosition(1, targetPoint);*/
        }

        _shootTimer = _shootInterval;
        StopCoroutine(nameof(HideRay));
        StartCoroutine(nameof(HideRay));
    }

    void HandleHit(RaycastHit weaponHit)
    {
        SpawnHitVFX(weaponHit);

        if (_hitParticle != null)
        {
            Instantiate(_hitParticle, weaponHit.point, Quaternion.LookRotation(weaponHit.normal));
        }

        if (weaponHit.collider.TryGetComponent<Enemy_Interface_Damage>(out var damageable))
            damageable.TakeDamage(_shootDamageAmount);
    }

    private IEnumerator HideRay()
    {
        _shootView.enabled = true;
        yield return new WaitForSeconds(_rayVisibleDuration);
        _shootView.enabled = false;
    }

    void ManageShoot()
    {
        _remainingBullets--;
        UpdateUI();
    }

    void Recharge(InputAction.CallbackContext context)
    {
        int totalReserve = Inventory.Instance.GetTotalAmmo();

        if (_remainingBullets == _maxBullets || totalReserve <= 0) return;

        int bulletsNeeded = _maxBullets - _remainingBullets;
        
        int bulletsObtained = Inventory.Instance.ConsumeAmmo(bulletsNeeded);

        _remainingBullets += bulletsObtained;
        
        UpdateUI();
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.RechargingGun);
    }

    void UpdateUI()
    {
        int currentReserve = Inventory.Instance.GetTotalAmmo();

        _remainingBulletsUI.text = $"{_remainingBullets}";
        _maxBulletsUI.text = $"{currentReserve}";
        
        _pressR.enabled = _remainingBullets < _maxBullets && currentReserve > 0;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float timer = 0f;

        while (timer < flashDuration)
        {
            timer += Time.deltaTime;

            float t = timer / flashDuration;
            flashLight.intensity = Mathf.Lerp(flashIntensity, 0f, t);

            yield return null;
        }

        flashLight.intensity = 0f;
        flashCoroutine = null;
    }

    private void SpawnHitVFX(RaycastHit weaponHit)
    {
        if (_defaultHitVFX == null)
            return;

        Vector3 spawnPosition = weaponHit.point + weaponHit.normal * 0.01f;

        Quaternion spawnRotation = Quaternion.FromToRotation(Vector3.forward, weaponHit.normal);

        VisualEffect hitVFX = Instantiate(
            _defaultHitVFX,
            spawnPosition,
            spawnRotation
        );

        hitVFX.Play();

        Destroy(hitVFX.gameObject, _hitVFXDestroyDelay);
    }

}