using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraTarget;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _groundCheckDistance = 3f;

    [Header("Camera")]
    [SerializeField] private float _mouseSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 40f;

    [Header("Aim Settings")]
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 40f;
    [SerializeField] private float _fovSpeed = 10f;

    private Rigidbody _rb;
    private Camera _camera;
    private PlayerControls _controls;
    private Quaternion _targetRotation;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _cameraPitch = 0f;
    private bool _isAiming;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _camera = _cameraTransform.GetComponent<Camera>();
        _controls = new PlayerControls();
        _targetRotation = _rb.rotation;

        _controls.Player.Move.performed += callbackContext => _moveInput = callbackContext.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _controls.Player.Look.performed += callbackContext => _lookInput = callbackContext.ReadValue<Vector2>();
        _controls.Player.Look.canceled += _ => _lookInput = Vector2.zero;

        _controls.Player.Aim.performed += _ => _isAiming = true;
        _controls.Player.Aim.canceled += _ => _isAiming = false;
    }

    private void OnEnable() => _controls.Enable();
    private void OnDisable() => _controls.Disable();

    private void Update()
    {
        HandleLook();
        HandleAim();
    }

    private void FixedUpdate()
    {
        ApplyRotation();
        HandleMovement();
    }

    void LateUpdate()
    {
        // So that the camera doesn't override it's rotation every frame
        _cameraTransform.rotation = _cameraTarget.rotation;
    }

    void HandleLook()
    {
        float mouseX = _lookInput.x * _mouseSensitivity;
        float mouseY = _lookInput.y * _mouseSensitivity;

        _targetRotation = _rb.rotation * Quaternion.Euler(0f, mouseX, 0f);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
        _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        Vector3 move = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        if (move.sqrMagnitude > 1f) move.Normalize();

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _groundCheckDistance))
        {
            // Follow the terrain
            move = Vector3.ProjectOnPlane(move, hit.normal).normalized * move.magnitude;
            _rb.linearVelocity = move * _moveSpeed;
        }
        else
        {
            // In the air, preserve gravity
            Vector3 targetVelocity = move * _moveSpeed;
            targetVelocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = targetVelocity;
        }
    }

    void HandleAim()
    {
        float targetFOV = _isAiming ? _aimFOV : _normalFOV;

        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * _fovSpeed);
    }

    void ApplyRotation()
    {
        _rb.MoveRotation(_targetRotation);
    }
}
