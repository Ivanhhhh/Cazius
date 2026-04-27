using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using System.Runtime.InteropServices.WindowsRuntime;


public class Player_AimAndShoot : MonoBehaviour
{
    [SerializeField] Camera _playerCamera;
    [SerializeField] PlayerMovement _movement;
    [SerializeField] Image _crossHair;
    [SerializeField] float _maxDistance;
    [SerializeField] int _totalReserveBullets;
    [SerializeField] private ParticleSystem _hitParticle;
     [SerializeField] int _maxBullets;
    [SerializeField] TextMeshProUGUI _maxBulletsUI;
    [SerializeField] TextMeshProUGUI _pressR;
    [SerializeField] private Player_CameraRecoil _recoil;
    [SerializeField] TextMeshProUGUI _remainingBulletsUI;
    [SerializeField] float _shootDamageAmount;
    private int _remainingBullets;
    private int _reserveBullets;    
    public bool _hasBullets => _remainingBullets > 0;
    bool a;
    void Start()
    {
        _crossHair.enabled = false;
        _remainingBullets = _maxBullets;
        _remainingBullets = _maxBullets;
        _reserveBullets = _totalReserveBullets;
        _movement._controls.Player.Recharge.started += Recharge;
        UpdateUI();
    }
    void Update()
    {
        if (_movement._controls.Player.Aim.IsPressed())
        {
            _crossHair.enabled = true;
            _movement._controls.Player.Shoot.started += OnShootStarted;
        }
        else
        {
            _crossHair.enabled = false;
            _movement._controls.Player.Shoot.started -= OnShootStarted;
        }
    }

    private void OnShootStarted(InputAction.CallbackContext context)
    {
        if (!_hasBullets) return;
        _recoil.OnRecoil?.Invoke();
        ManageShoot();

        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;
       
        Vector3 targetPoint;

        if (Physics.Raycast(cameraRay, out hit, _maxDistance))
            targetPoint = hit.point;
        else
            targetPoint = cameraRay.origin + cameraRay.direction * _maxDistance;
        Vector3 direction = (targetPoint - transform.position).normalized;
        
        Debug.DrawRay(transform.position, direction * _maxDistance, Color.red);

        RaycastHit weaponHit;
        if (Physics.Raycast(transform.position, direction, out weaponHit, _maxDistance))
        {

            // Particle Spawn
            if (_hitParticle != null)
            {
                Instantiate(
                    _hitParticle,
                    weaponHit.point,
                    Quaternion.LookRotation(weaponHit.normal)
                );
            }
            if (hit.collider.TryGetComponent<Enemy_Interface_Damage>(out var damageable))
            {
                damageable.TakeDamage (_shootDamageAmount);
            }
        }
    }
    void ManageShoot()
    {
        _remainingBullets --;
        _remainingBulletsUI.text = $"{_remainingBullets}";
    }
    void Recharge(InputAction.CallbackContext context)
    {
        if (_remainingBullets == _maxBullets) return;
        if (_reserveBullets <= 0) return;
        int bulletsNeeded = _maxBullets - _remainingBullets;
        int bulletsToAdd = Mathf.Min(bulletsNeeded, _reserveBullets);
        _remainingBullets += bulletsToAdd;
        _reserveBullets -= bulletsToAdd;
        UpdateUI();
    }
    void UpdateUI()
    {
        _remainingBulletsUI.text = $"{_remainingBullets}";
        _maxBulletsUI.text = $"{_reserveBullets}"; // muestra la reserva
        _pressR.enabled = _remainingBullets < _maxBullets && _reserveBullets > 0;
    }
}
