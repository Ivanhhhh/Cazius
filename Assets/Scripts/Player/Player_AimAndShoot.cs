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
    [SerializeField] private VisualEffect _shootMuzzleVFX2;
    [SerializeField] private LineRenderer _shootView;

    [Header("Ammo")]
    [SerializeField] private int _maxBullets;
    [SerializeField] private int _totalReserveBullets;

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

    [Header("Blood VFX Graph")]
    [SerializeField] private VisualEffect _bloodHitVFX;
    [SerializeField] private float _bloodVFXDestroyDelay = 2f;
    [SerializeField] private float _bloodSurfaceOffset = 0.03f;
     private string _bloodPlayEventName = "OnPlay";

    [Header("Decals")]
    [SerializeField] private BulletDecalSpawner _bulletDecalSpawner;

    private int _remainingBullets;
    private int _reserveBullets;
    private float _shootTimer;
    public float _currentSpread;

    public bool _hasBullets => _remainingBullets > 0;

    void Start()
    {
        _shootTimer = _shootInterval;
        _remainingBullets = _maxBullets;
        _reserveBullets = _totalReserveBullets;

        _crossHair.enabled = false;
        _shootView.enabled = false;
        _crosshairTop.gameObject.SetActive(false);
        _crosshairBottom.gameObject.SetActive(false);
        _crosshairLeft.gameObject.SetActive(false);
        _crosshairRight.gameObject.SetActive(false);

        GameInputManager.Instance.Controls.Player.Recharge.started += Recharge;
        UpdateUI();

        if (flashLight == null)
            flashLight = GetComponent<Light>();

        flashLight.intensity = 0f;
        flashLight.range = flashRange;
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
        if (GameInputManager.Instance.Controls.Player.Aim.IsPressed())
            GameInputManager.Instance.Controls.Player.Shoot.started += OnShootStarted;
        else
            GameInputManager.Instance.Controls.Player.Shoot.started -= OnShootStarted;
    }

    void UpdateCrosshairIndicators()
    {
        bool isAiming = GameInputManager.Instance.Controls.Player.Aim.IsPressed();

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
        _shootMuzzleVFX2.SendEvent("OnPlay");
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
        bool hitEnemy = weaponHit.collider.TryGetComponent<Enemy_Interface_Damage>(out var damageable);

        if (_bulletDecalSpawner != null)
        {
            if (hitEnemy)
            {
                _bulletDecalSpawner.SpawnBloodyDecal(weaponHit);
            }
            else
            {
                _bulletDecalSpawner.SpawnNormalDecal(weaponHit);
            }
        }

        if (hitEnemy)
        {
            SpawnBloodVFXGraph(weaponHit);
            damageable.TakeDamage(_shootDamageAmount);
        }
        else
        {
            SpawnHitVFX(weaponHit);

            if (_hitParticle != null)
            {
                Instantiate(_hitParticle,weaponHit.point,Quaternion.LookRotation(weaponHit.normal));
            }
        }
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
        if (_remainingBullets == _maxBullets || _reserveBullets <= 0) return;

        // Toma solo las balas necesarias para no exceder el máximo del cargador
        int bulletsToAdd = Mathf.Min(_maxBullets - _remainingBullets, _reserveBullets);
        _remainingBullets += bulletsToAdd;
        _reserveBullets -= bulletsToAdd;
        UpdateUI();
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.RechargingGun);
    }

    void UpdateUI()
    {
        _remainingBulletsUI.text = $"{_remainingBullets}";
        _maxBulletsUI.text = $"{_reserveBullets}";
        _pressR.enabled = _remainingBullets < _maxBullets && _reserveBullets > 0;
    }

    public void AddReserveBullets(int bulletsToAdd)
    {
        _reserveBullets += bulletsToAdd;
        if (_reserveBullets >= _totalReserveBullets) _reserveBullets = _totalReserveBullets;
        SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.RechargingGun);
        UpdateUI();
    }

    //flashlight effect, cambiar a otro script en algun momento
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

    private void SpawnBloodVFXGraph(RaycastHit weaponHit)
    {
        if (_bloodHitVFX == null)
            return;

        Vector3 spawnPosition = weaponHit.point + weaponHit.normal * _bloodSurfaceOffset;

        Quaternion spawnRotation = Quaternion.LookRotation(weaponHit.normal);

        VisualEffect bloodVFX = Instantiate(
            _bloodHitVFX,
            spawnPosition,
            spawnRotation
        );

        bloodVFX.SendEvent(_bloodPlayEventName);

        Destroy(bloodVFX.gameObject, _bloodVFXDestroyDelay);
    }

}