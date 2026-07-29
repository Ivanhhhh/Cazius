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

    [Header("Camera Elevator Logic")]
    [SerializeField] private float _rotationCenterY = 1.2f;
    [SerializeField] private float _rotationCenterZ = 0.2f;
    [SerializeField] private float _cameraXOffset = 0.4f;

    [Header("Topes de Altura (Y)")]
    [SerializeField] private float _minCameraY = -0.2f;
    [SerializeField] private float _maxCameraY = 1.2f;

    [Header("Topes de Profundidad (Z)")]
    [SerializeField] private float _minCameraZ = -1.5f;
    [SerializeField] private float _maxCameraZ = -0.5f;

    [Header("Camera Settings")]
    [SerializeField] private float _sphereCollisionRadius = 0.2f;
    [SerializeField] private LayerMask _cameraCollisionMask;
    [SerializeField] private float _mouseSensitivityY = 0.15f;
    [SerializeField] private float _mouseSensitivityX = 0.75f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 40f;
    [SerializeField] private float _cameraVerticalTilt = 0.3f;

    [Header("Camera Target Dynamic")]
    [SerializeField] private float _minTargetY = 0.2f;
    [SerializeField] private float _maxTargetY = 0.8f;

    // --- NEW ---
    [Header("Camera Collision Response")]
    [SerializeField] private float _xOffsetShiftSpeed = 10f;  // How fast shoulder shifts on wall hit
    [SerializeField] private float _xOffsetReturnSpeed = 5f;  // How fast shoulder recovers when clear

    [Header("Aim Settings")]
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 40f;
    [SerializeField] private float _fovSpeed = 10f;
    [SerializeField] private Player_CameraRecoil _recoil;

    [Header("Crosshair Aim Target")]
    [SerializeField] private Transform _targetObject;

    [SerializeField] private LayerMask _aimCollisionMask;

    [SerializeField] private float _maximumAimDistance = 100f;

    [SerializeField] private float _fallbackAimDistance = 30f;

    [SerializeField] private float _followSpeed = 25f;

    [SerializeField] private float _minimumVisualAimDistance = 10f;

    private Vector3 _currentAimPoint;

    public Vector3 CurrentAimPoint => _currentAimPoint;

    private Vector3 _visualAimPoint;
    private Vector3 _actualAimPoint;

    public Vector3 ActualAimPoint => _actualAimPoint;

    [Header("Visual Aim Alignment")]
    [SerializeField] private Transform _modelAimPivot;

    [SerializeField] private float _aimYawOffset = 8f;

    [SerializeField] private float _aimYawRotationSpeed = 12f;

    private Quaternion _modelAimDefaultLocalRotation;

    [Header("Visual Aim Target Offset")]
    [SerializeField] private float _aimTargetYawOffset = 5f;

    [SerializeField] private float _aimTargetYawSpeed = 10f;

    private float _currentAimTargetYawOffset;

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

    // --- NEW: runtime X offset that gets lerped ---
    private float _currentXOffset;

    public float _currentSpeed { get; set; }

    private void Awake()
    {
        _camera = _cameraTransform.GetComponent<Camera>();
        _rb = GetComponent<Rigidbody>();
        _aimSpeed = _moveSpeed / 2;
        _targetRotation = _rb.rotation;
        _currentXOffset = _cameraXOffset;   // start at the configured shoulder
        _modelAimDefaultLocalRotation = _modelAimPivot.localRotation;
    }

    private void Start()
    {
        _controls = GameInputManager.Instance.Controls;

        _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _controls.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += _ => _lookInput = Vector2.zero;

        _controls.Player.Aim.performed += _ => _isAiming = true;
        _controls.Player.Aim.canceled += _ => _isAiming = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        HandleAim();
        _smoothedMoveInput = Vector2.Lerp(_smoothedMoveInput, _moveInput, Time.deltaTime * 10f);
    }

    private void FixedUpdate()
    {
        ApplyRotation();
        HandleMovement();
    }

    void LateUpdate()
    {
        // ── 1. Pivot (chest height + forward offset) ─────────────────────────
        Vector3 pivotPosition = transform.position
                              + Vector3.up * _rotationCenterY
                              + transform.forward * _rotationCenterZ;

        // ── 2. Pitch-driven Y / Z offsets ────────────────────────────────────
        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);
        float dynamicYOffset = Mathf.Lerp(_minCameraY, _maxCameraY, pitchT);
        float dynamicZOffset = Mathf.Lerp(_minCameraZ, _maxCameraZ, pitchT);

        // ── Phase 1 : determine target X offset (shoulder center / flip) ──────
        //
        // Cheap point-raycasts to decide which shoulder position is clear.
        // Priority: original shoulder → center → flipped shoulder.
        // We only use point casts here (not sphere) because we just want to
        // know the general direction, not compute exact camera placement.

        float targetXOffset = _cameraXOffset;   // assume original is free

        Vector3 desiredFull = pivotPosition
                            + transform.right * _cameraXOffset
                            + Vector3.up * dynamicYOffset
                            + transform.forward * dynamicZOffset;

        Vector3 dirFull = desiredFull - pivotPosition;

        if (Physics.Raycast(pivotPosition, dirFull.normalized, dirFull.magnitude, _cameraCollisionMask))
        {
            // Original shoulder is blocked → try center (X = 0)
            targetXOffset = 0f;

            Vector3 desiredCenter = pivotPosition
                                  + Vector3.up * dynamicYOffset
                                  + transform.forward * dynamicZOffset;

            Vector3 dirCenter = desiredCenter - pivotPosition;

            if (Physics.Raycast(pivotPosition, dirCenter.normalized, dirCenter.magnitude, _cameraCollisionMask))
            {
                // Center also blocked → flip to opposite shoulder
                targetXOffset = -_cameraXOffset;
            }
        }

        // Lerp toward target: shift fast, recover slow (feels more responsive on entry)
        float lerpSpeed = (targetXOffset == _cameraXOffset) ? _xOffsetReturnSpeed : _xOffsetShiftSpeed;
        _currentXOffset = Mathf.Lerp(_currentXOffset, targetXOffset, Time.deltaTime * lerpSpeed);

        // ── Phase 2 : SphereCast for Z pull-in using the lerped X offset ─────
        //
        // This handles walls directly behind the player after X is already resolved.
        // Mathf.Max(0, ...) prevents the camera snapping to the pivot when the
        // sphere literally cannot fit — it stays as close as physically possible.

        Vector3 desiredPosition = pivotPosition
                                + transform.right * _currentXOffset
                                + Vector3.up * dynamicYOffset
                                + transform.forward * dynamicZOffset;

        Vector3 direction = desiredPosition - pivotPosition;
        float distance = direction.magnitude;

        if (Physics.SphereCast(pivotPosition, _sphereCollisionRadius, direction.normalized,
                                out RaycastHit hit, distance, _cameraCollisionMask))
        {
            float safeDistance = Mathf.Max(0f, hit.distance - _sphereCollisionRadius);
            _cameraTransform.position = pivotPosition + direction.normalized * safeDistance;
        }
        else
        {
            _cameraTransform.position = desiredPosition;
        }

        // ── 3. Orientation + recoil ───────────────────────────────────────────
        _cameraTransform.LookAt(_cameraTarget.position);
        _cameraTransform.rotation *= Quaternion.Euler(_recoil.CurrentRotation);

        // ── 4. Rig aim target ─────────────────────────────────────────────────
        UpdateAimTarget();

        HandleVisualAimRotation();
    }

    void HandleLook()
    {
        float mouseX = _lookInput.x * _mouseSensitivityX;
        float mouseY = _lookInput.y * _mouseSensitivityY;

        _yaw += mouseX;

        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
        _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);
        float dynamicTargetY = Mathf.Lerp(_minTargetY, _maxTargetY, pitchT);

        Vector3 localPos = _cameraTarget.localPosition;
        localPos.y = dynamicTargetY;
        _cameraTarget.localPosition = localPos;
    }

    void HandleMovement()
    {
        float actualSpeed = _isAiming ? _aimSpeed : _moveSpeed;

        Vector3 camForward = _cameraTransform.forward;
        Vector3 camRight = _cameraTransform.right;

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

            _animator.SetBool("IsAiming", _isAiming);

        _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, targetFOV, Time.deltaTime * _fovSpeed);
    }

    void ApplyRotation()
    {
        _rb.MoveRotation(Quaternion.Euler(0f, _yaw, 0f));
    }

    public void ResetInput()
    {
        _moveInput = Vector2.zero;
        _lookInput = Vector2.zero;

        Debug.Log(_moveInput);
    }

    private void OnDrawGizmos()
    {
        // Fixed: now includes _rotationCenterZ (was missing in the original)
        Vector3 pivotPosition = transform.position
                              + Vector3.up * _rotationCenterY
                              + transform.forward * _rotationCenterZ;

        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);
        float dynamicYOffset = Mathf.Lerp(_minCameraY, _maxCameraY, pitchT);
        float dynamicZOffset = Mathf.Lerp(_minCameraZ, _maxCameraZ, pitchT);

        // In play mode reflect the live lerped offset; in editor show the configured default
        float xOffset = Application.isPlaying ? _currentXOffset : _cameraXOffset;

        Vector3 desiredPosition = pivotPosition
                                + transform.right * xOffset
                                + Vector3.up * dynamicYOffset
                                + transform.forward * dynamicZOffset;

        Vector3 direction = desiredPosition - pivotPosition;
        float distance = direction.magnitude;

        if (Physics.SphereCast(pivotPosition, _sphereCollisionRadius, direction.normalized,
                                out RaycastHit hit, distance, _cameraCollisionMask))
        {
            Vector3 clampedPosition = pivotPosition
                                    + direction.normalized
                                    * Mathf.Max(0f, hit.distance - _sphereCollisionRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(pivotPosition, clampedPosition);

            Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
            Gizmos.DrawSphere(clampedPosition, _sphereCollisionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(clampedPosition, _sphereCollisionRadius);

            Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
            Gizmos.DrawLine(clampedPosition, desiredPosition);
            Gizmos.DrawWireSphere(desiredPosition, _sphereCollisionRadius);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pivotPosition, desiredPosition);

            Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
            Gizmos.DrawSphere(desiredPosition, _sphereCollisionRadius);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(desiredPosition, _sphereCollisionRadius);
        }

        // Pivot
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivotPosition, 0.05f);

    }

    private void HandleVisualAimRotation()
    {
        if (_modelAimPivot == null)
            return;

        float yawOffset = _isAiming ? _aimYawOffset : 0f;

        Quaternion desiredRotation =
            _modelAimDefaultLocalRotation *
            Quaternion.Euler(0f, yawOffset, 0f);

        float interpolation =
            1f - Mathf.Exp(-_aimYawRotationSpeed * Time.deltaTime);

        _modelAimPivot.localRotation = Quaternion.Slerp(
            _modelAimPivot.localRotation,
            desiredRotation,
            interpolation
        );
    }

    private void UpdateAimTarget()
    {
        if (_targetObject == null || _camera == null)
            return;

        Ray crosshairRay = _camera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        float fallbackDistance = Mathf.Max(
            _fallbackAimDistance,
            _minimumVisualAimDistance
        );

        Vector3 fallbackPoint =
            crosshairRay.GetPoint(fallbackDistance);

        _actualAimPoint = fallbackPoint;
        _visualAimPoint = fallbackPoint;

        if (Physics.Raycast(
            crosshairRay,
            out RaycastHit hit,
            _maximumAimDistance,
            _aimCollisionMask,
            QueryTriggerInteraction.Ignore))
        {
            _actualAimPoint = hit.point;

            float distanceFromPlayer =
                Vector3.Distance(transform.position, hit.point);

            if (distanceFromPlayer >= _minimumVisualAimDistance)
            {
                _visualAimPoint = hit.point;
            }
        }

        float interpolation =
            1f - Mathf.Exp(-_followSpeed * Time.deltaTime);

        Vector3 adjustedVisualAimPoint =
            ApplyVisualAimTargetOffset(_visualAimPoint);

        _targetObject.position = Vector3.Lerp(
            _targetObject.position,
            adjustedVisualAimPoint,
            interpolation
        );
    }

    private Vector3 ApplyVisualAimTargetOffset(Vector3 originalAimPoint)
    {
        float desiredYawOffset = _isAiming
            ? _aimTargetYawOffset
            : 0f;

        float interpolation =
            1f - Mathf.Exp(-_aimTargetYawSpeed * Time.deltaTime);

        _currentAimTargetYawOffset = Mathf.Lerp(
            _currentAimTargetYawOffset,
            desiredYawOffset,
            interpolation
        );

        Vector3 rotationPivot = _modelAimPivot != null
            ? _modelAimPivot.position
            : transform.position;

        Vector3 directionToTarget =
            originalAimPoint - rotationPivot;

        if (directionToTarget.sqrMagnitude < 0.001f)
            return originalAimPoint;

        Quaternion yawRotation = Quaternion.AngleAxis(
            _currentAimTargetYawOffset,
            transform.up
        );

        return rotationPivot +
               yawRotation * directionToTarget;
    }
}