using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float mouseSensitivity = 2f;

    private PlayerControls controls;
    private Vector2 moveInput;
    private Vector2 lookInput;

    private float cameraPitch = 0f;

    private void Awake()
    {
        controls = new PlayerControls();

        controls.Player.Move.performed += callbackContext => moveInput = callbackContext.ReadValue<Vector2>();
        controls.Player.Move.canceled += _ => moveInput = Vector2.zero;

        controls.Player.Look.performed += callbackContext => lookInput = callbackContext.ReadValue<Vector2>();
        controls.Player.Look.canceled += _ => lookInput = Vector2.zero;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    void HandleLook()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);
       
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -70f, 70f);

        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        Vector3 move = forward * moveInput.y + right * moveInput.x;

        if (move.magnitude > 0.1f)
        {
           
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position += move.normalized * moveSpeed * Time.deltaTime;
    }
}
