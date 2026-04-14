using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Player_AimAndShoot : MonoBehaviour
{
    [SerializeField] Camera _playerCamera;
    [SerializeField] PlayerMovement _movement;
    [SerializeField] Image _crossHair;
    void Start()
    {
        _crossHair.enabled = false;
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
        Ray cameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;
       
        Vector3 targetPoint;

        if (Physics.Raycast(cameraRay, out hit, 100f))
            targetPoint = hit.point;
        else
            targetPoint = cameraRay.origin + cameraRay.direction * 100f;
        Vector3 direction = (targetPoint - transform.position).normalized;
        
        Debug.DrawRay(transform.position, direction * 100f, Color.red);

        RaycastHit weaponHit;
        if (Physics.Raycast(transform.position, direction, out weaponHit, 100f))
        {
            if (hit.collider.TryGetComponent<Enemy_HealthSystem>(out var enemy))
            {
                Destroy(hit.collider.gameObject);
            }
        }
    }
}
