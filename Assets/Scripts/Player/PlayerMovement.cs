using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraTarget;

    [Header("Movement")]
    [SerializeField] public float _moveSpeed = 10f;

    [Header("Camera")]
    [SerializeField] private float _mouseSensitivity = 0.15f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 40f;

    [Header("Aim Settings")]
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 40f;
    [SerializeField] private float _fovSpeed = 10f;

    public PlayerControls _controls;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _cameraPitch = 0f;
    private bool _isAiming;

    private void Awake()
    {
        _controls = new PlayerControls();

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
        HandleMovement();
        HandleAim();
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

        transform.Rotate(Vector3.up * mouseX);

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);

        _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = forward * _moveInput.y + right * _moveInput.x;

        transform.position += move.normalized * _moveSpeed * Time.deltaTime;
    }

    void HandleAim()
    {
        float targetFOV = _isAiming ? _aimFOV : _normalFOV;

        Camera cam = _cameraTransform.GetComponent<Camera>();
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.deltaTime * _fovSpeed);
    }
}
