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
    [SerializeField] private float _rotationCenterY = 1.2f; // Altura del pecho/cabeza (Pivote central)
    [SerializeField] private float _rotationCenterZ = 0.2f; // NUEVO: Desplazamiento del centro hacia adelante/atrás
    [SerializeField] private float _cameraXOffset = 0.4f;   // Distancia lateral FIJA

    [Header("Topes de Altura (Y)")]
    [SerializeField] private float _minCameraY = -0.2f; // Altura mínima (Se alcanza al llegar a _minPitch)
    [SerializeField] private float _maxCameraY = 1.2f;  // Altura máxima (Se alcanza al llegar a _maxPitch)

    [Header("Topes de Profundidad (Z)")]
    [SerializeField] private float _minCameraZ = -1.5f; // Qué tan lejos está al mirar arriba (Valores negativos)
    [SerializeField] private float _maxCameraZ = -0.5f; // Qué tan cerca está al mirar abajo

    [Header("Camera Settings")]
    [SerializeField] private float _cameraCollisionRadius = 0.2f;
    [SerializeField] private LayerMask _cameraCollisionMask;
    [SerializeField] private float _mouseSensitivityY = 0.15f;
    [SerializeField] private float _mouseSensitivityX = 0.75f;
    [SerializeField] private float _minPitch = -40f;
    [SerializeField] private float _maxPitch = 40f;
    [SerializeField] private float _cameraVerticalTilt = 0.3f;

    [Header("Camera Target Dynamic")]
    [SerializeField] private float _minTargetY = 0.2f; // Altura del target al mirar arriba (Pitch mínimo)
    [SerializeField] private float _maxTargetY = 0.8f; // Altura del target al mirar abajo (Pitch máximo)

    [Header("Aim Settings")]
    [SerializeField] private float _normalFOV = 60f;
    [SerializeField] private float _aimFOV = 40f;
    [SerializeField] private float _fovSpeed = 10f;
    [SerializeField] private Player_CameraRecoil _recoil;
    
    [Header("Rig Object")]
    [SerializeField] private Transform _targetObject; 
    [SerializeField] private float _maxDistance = 5f; 
    [SerializeField] private float _followSpeed = 15f; 

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

    public float _currentSpeed { get; set; }

    private void Awake()
    {
        _camera = _cameraTransform.GetComponent<Camera>();
        _rb = GetComponent<Rigidbody>();
        _aimSpeed = _moveSpeed / 2;
        _targetRotation = _rb.rotation;
    }

    private void Start()
    {
        _controls = GameInputManager.Instance.Controls;

        _controls.Player.Move.performed += callbackContext => _moveInput = callbackContext.ReadValue<Vector2>();
        _controls.Player.Move.canceled += _ => _moveInput = Vector2.zero;

        _controls.Player.Look.performed += callbackContext => _lookInput = callbackContext.ReadValue<Vector2>();
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
        // 1. Establecemos el pivote central (Ej: Altura del pecho)
        Vector3 pivotPosition = transform.position 
                    + Vector3.up * _rotationCenterY 
                    + transform.forward * _rotationCenterZ;

        // 2. Calculamos el porcentaje de rotación actual (0 a 1)
        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);

        // 3. Calculamos la altura (Y) y la profundidad (Z) dinámica interpolando entre los topes fijos
        float dynamicYOffset = Mathf.Lerp(_minCameraY, _maxCameraY, pitchT);
        float dynamicZOffset = Mathf.Lerp(_minCameraZ, _maxCameraZ, pitchT);

        // 4. Posición ideal usando X fijo, Y dinámico, Z dinámico
        Vector3 desiredPosition = pivotPosition
            + transform.right * _cameraXOffset
            + Vector3.up * dynamicYOffset
            + transform.forward * dynamicZOffset;

        Vector3 direction = desiredPosition - pivotPosition;
        float distance = direction.magnitude;

        // El Raycast se dispara desde el pecho (pivote) hacia la cámara
        if (Physics.Raycast(pivotPosition, direction.normalized, out RaycastHit hit, distance, _cameraCollisionMask))
            _cameraTransform.position = hit.point;
        else
            _cameraTransform.position = desiredPosition;

        _cameraTransform.LookAt(_cameraTarget.position);
        _cameraTransform.rotation *= Quaternion.Euler(_recoil.CurrentRotation);

        if (_targetObject != null)
        {
            Vector3 targetPosition = _cameraTransform.position + _cameraTransform.forward * _maxDistance;
            _targetObject.position = Vector3.Lerp(_targetObject.position, targetPosition, Time.deltaTime * _followSpeed);
        }
    }

    void HandleLook()
    {
        float mouseX = _lookInput.x * _mouseSensitivityX;
        float mouseY = _lookInput.y * _mouseSensitivityY;

        _yaw += mouseX;

        // 1. Calculamos y limitamos la rotación (El Pitch)
        _cameraPitch -= mouseY;
        _cameraPitch = Mathf.Clamp(_cameraPitch, _minPitch, _maxPitch);
        _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);

        // 2. Porcentaje de inclinación (0 a 1)
        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);

        // 3. Calculamos la altura dinámica del objetivo
        float dynamicTargetY = Mathf.Lerp(_minTargetY, _maxTargetY, pitchT);

        // 4. Aplicamos la nueva altura al _cameraTarget
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
    }

    private void OnDrawGizmos()
    {
        Vector3 pivotPosition = transform.position + Vector3.up * _rotationCenterY;
        
        float pitchT = Mathf.InverseLerp(_minPitch, _maxPitch, _cameraPitch);
        float dynamicYOffset = Mathf.Lerp(_minCameraY, _maxCameraY, pitchT);
        float dynamicZOffset = Mathf.Lerp(_minCameraZ, _maxCameraZ, pitchT);

        Vector3 desiredPosition = pivotPosition
            + transform.right * _cameraXOffset
            + Vector3.up * dynamicYOffset
            + transform.forward * dynamicZOffset;

        Vector3 direction = desiredPosition - pivotPosition;
        float distance = direction.magnitude;

        if (Physics.Raycast(pivotPosition, direction.normalized, out RaycastHit hit, distance, _cameraCollisionMask))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pivotPosition, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.1f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pivotPosition, desiredPosition);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(desiredPosition, 0.1f);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pivotPosition, 0.05f);
    }
}