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
    [SerializeField] private ParticleSystem _hitParticle;
     [SerializeField] int _maxBullets;
    [SerializeField] TextMeshProUGUI _maxBulletsUI;
    [SerializeField] TextMeshProUGUI _pressR;
    private int _remainingBullets;
    [SerializeField] TextMeshProUGUI _remainingBulletsUI;
    public bool _hasBullets => _remainingBullets > 0;
    bool a;
    void Start()
    {
        _crossHair.enabled = false;
        _remainingBullets = _maxBullets;
        _maxBulletsUI.text = $"{_maxBullets}";
        _remainingBulletsUI.text = $"{_remainingBullets}";
        _movement._controls.Player.Recharge.started += Recharge;

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
            if (hit.collider.TryGetComponent<Enemy_HealthSystem>(out var enemy))
            {
                Destroy(hit.collider.gameObject);
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
        _remainingBullets = _maxBullets;
        _remainingBulletsUI.text = $"{_remainingBullets}";
    }
}
