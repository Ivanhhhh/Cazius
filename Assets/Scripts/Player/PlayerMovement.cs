using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _cameraTarget;
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField] public float _moveSpeed = 10f;
    [SerializeField] private float _groundingForce = 20f;
    [SerializeField] private float _groundCheckDistance = 0.1f;

    [Header("Camera")]
    [SerializeField] private float _cameraDistance = 1f;
    [SerializeField] private float _cameraXOffset = 0.4f;
    [SerializeField] private float _cameraYOffset = 0.4f;
    [SerializeField] private float _cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask _cameraCollisionMask;
    [SerializeField] private float _mouseSensitivityY = 0.15f;
    [SerializeField] private float _mouseSensitivityX = 0.75f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 40f;
    [SerializeField] private float _cameraVerticalTilt = 0.3f;

    [Header("Camera Target")]
    [SerializeField] private float _YOffset = 0.4f;
    [SerializeField] private float _minVerticalY = -1f;
    [SerializeField] private float _maxVerticalY = 1f;

    [Header("Aim Settings")]
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 40f;
    [SerializeField] private float _fovSpeed = 10f;
    [SerializeField] private Player_CameraRecoil _recoil;
    private Rigidbody _rb;
    private Camera _camera;
    public PlayerControls _controls;
    private Quaternion _targetRotation;
    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private float _cameraPitch = 0f;
    private bool _isAiming;
    private float _aimSpeed;
    private float _yaw;
    private Vector2 _smoothedMoveInput;
    float _verticalOffset;

    public float _currentSpeed {get;set;}
    private void Awake()
    {
        _aimSpeed = _moveSpeed / 2;
        _rb = GetComponent<Rigidbody>();
        _camera = _cameraTransform.GetComponent<Camera>();
        _controls = new PlayerControls();
        Cursor.lockState = CursorLockMode.Locked;
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
        // anims
        _smoothedMoveInput = Vector2.Lerp(_smoothedMoveInput, _moveInput, Time.deltaTime * 10f);
    }

    private void FixedUpdate()
    {
        ApplyRotation();
        HandleMovement();
    }

    void LateUpdate()
    {
        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);
        //float cameraVertical = Mathf.Lerp(_cameraVerticalTilt, -_cameraVerticalTilt, pitchT);

        Vector3 desiredPosition = transform.position
            + transform.right * _cameraXOffset
            + Vector3.up * (_cameraYOffset)
            + transform.forward * -_cameraDistance;

        Vector3 direction = desiredPosition - transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, _cameraCollisionMask))
            _cameraTransform.position = hit.point;
        else
            _cameraTransform.position = desiredPosition;

        _cameraTransform.LookAt(_cameraTarget.position);
        _cameraTransform.rotation *= Quaternion.Euler(_recoil.CurrentRotation);
    }

    void HandleLook()
    {
        float mouseX = _lookInput.x * _mouseSensitivityX;
        float mouseY = _lookInput.y * _mouseSensitivityY;

        _verticalOffset -= -mouseY * 2f * Time.deltaTime;
        _verticalOffset = Mathf.Clamp(_verticalOffset, _minVerticalY, _maxVerticalY);

        //_targetRotation = _rb.rotation * Quaternion.Euler(0f, mouseX, 0f);
        _yaw += mouseX;

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
        _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

        Vector3 localPos = _cameraTarget.localPosition;
        localPos.y = _verticalOffset+ _YOffset;
        _cameraTarget.localPosition = localPos;
    }
    /*
    void HandleMovement()
    {
        float actualSpeed = _isAiming ? _aimSpeed : _moveSpeed;
        Vector3 move = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        if (move.sqrMagnitude > 1f) move.Normalize();

        bool grounded = Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance);

        _rb.linearVelocity = new Vector3(move.x * actualSpeed, _rb.linearVelocity.y, move.z * actualSpeed);
        _rb.AddForce(Vector3.down * _groundingForce, ForceMode.Force);

        // Animator
        bool isMoving = _smoothedMoveInput.sqrMagnitude > 0.01f;

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetFloat("MoveX", _smoothedMoveInput.y, 0.3f, Time.deltaTime);
        _animator.SetFloat("MoveZ", _smoothedMoveInput.x, 0.3f, Time.deltaTime);

        _currentSpeed = _rb.linearVelocity.magnitude;
    }*/
    void HandleMovement()
    {
        float actualSpeed = _isAiming ? _aimSpeed : _moveSpeed;

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

        // Remove vertical angle so movement stays flat
        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = camForward * _moveInput.y + camRight * _moveInput.x;

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        _rb.linearVelocity = new Vector3(
            move.x * actualSpeed,
            _rb.linearVelocity.y,
            move.z * actualSpeed
        );

        _rb.AddForce(Vector3.down * _groundingForce, ForceMode.Force);

        bool isMoving = _smoothedMoveInput.sqrMagnitude > 0.01f;

        _animator.SetBool("IsMoving", isMoving);
        _animator.SetFloat("MoveX", _smoothedMoveInput.y, 0.3f, Time.deltaTime);
        _animator.SetFloat("MoveZ", _smoothedMoveInput.x, 0.3f, Time.deltaTime);

        _currentSpeed = _rb.linearVelocity.magnitude;
    }

    void HandleAim()
    {
        float targetFOV = _isAiming ? _aimFOV : _normalFOV;
        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * _fovSpeed);
    }

    void ApplyRotation()
    {
        //_rb.MoveRotation(_targetRotation);
        _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));
    }

    private void OnDrawGizmos()
    {
        if (_cameraTarget == null) return;

        Vector3 desiredPosition = _cameraTarget.position
            + _cameraTarget.forward * -_cameraDistance
            + _cameraTarget.right * _cameraXOffset
            + _cameraTarget.up * _cameraYOffset;

        Vector3 direction = desiredPosition - transform.position;
        float distance = direction.magnitude;

        if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, distance, _cameraCollisionMask))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, desiredPosition);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(desiredPosition, 0.1f);
    }
}
