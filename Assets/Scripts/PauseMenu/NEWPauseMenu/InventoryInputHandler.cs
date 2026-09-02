using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections;

public class InventoryInputHandler : MonoBehaviour
{


    [SerializeField] private GameObject _inventoryCanvas;
    [SerializeField] private InventoryFadeController _fadeController;

    [SerializeField] private Transform _cameraParent;

    [SerializeField] private Vector3 _inventoryPositionOffset;
    [SerializeField] private Vector3 _inventoryRotationOffset;

    private Vector3 _normalCameraPosition;
    private Quaternion _normalCameraRotation;

    [SerializeField] private float _cameraTransitionDuration = 0.45f;

    [SerializeField] private AnimationCurve _cameraCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private PlayerMovement _cameraLookController;

    public static event Action<bool> OnInventoryToggled;
    public static event Action<bool> OnInventoryVisibilityChanged;

    private bool _inventoryOpen;
    private bool _transitioning;

    private Vector3 _cameraPositionBeforeInventory;
    private Quaternion _cameraRotationBeforeInventory;


    void Start() => GameInputManager.Instance.Controls.UI.InventoryMenu.performed += OnInventory;

    void OnDisable() => GameInputManager.Instance.Controls.UI.InventoryMenu.performed -= OnInventory;

    private void OnInventory(InputAction.CallbackContext _)
    {
        /*
        if (_inventoryCanvas.activeSelf)
        {
            PauseManager.Instance.Toggle();
            _inventoryCanvas.SetActive(false);

            OnInventoryToggled?.Invoke(false);
            return;
        }

        if (PauseManager.Instance.IsPaused) return;

        PauseManager.Instance.Toggle();
        _inventoryCanvas.SetActive(true);

        OnInventoryToggled?.Invoke(true);
        */
        if (_transitioning)
            return;

        if (_inventoryOpen)
        {
            StartCoroutine(CloseInventory());
            return;
        }

        if (PauseManager.Instance.IsPaused)
            return;

        StartCoroutine(OpenInventory());
    }

    private IEnumerator OpenInventory()
    {
        _transitioning = true;

        _cameraPositionBeforeInventory = _cameraParent.localPosition;
        _cameraRotationBeforeInventory = _cameraParent.localRotation;

        OnInventoryVisibilityChanged?.Invoke(true);

        if (WorldScanManager.Instance != null)
            WorldScanManager.Instance.SetInventoryActive(true);

        if (_cameraLookController != null)
            _cameraLookController.BeginInventoryCamera();

        _inventoryCanvas.SetActive(true);
        _fadeController.SetFade(0f);

        yield return StartCoroutine(TransitionCamera(true));

        PauseManager.Instance.Toggle();

        _inventoryOpen = true;
        _transitioning = false;

        OnInventoryToggled?.Invoke(true);
    }

    private IEnumerator CloseInventory()
    {
        _transitioning = true;

        PauseManager.Instance.Toggle();

        OnInventoryVisibilityChanged?.Invoke(false);

        yield return StartCoroutine(TransitionCamera(false));

        _inventoryCanvas.SetActive(false);

        if (_cameraLookController != null)
            _cameraLookController.EndInventoryCamera();

        if (WorldScanManager.Instance != null)
            WorldScanManager.Instance.SetInventoryActive(false);

        _inventoryOpen = false;
        _transitioning = false;

        OnInventoryToggled?.Invoke(false);
    }

    private IEnumerator TransitionCamera(bool opening)
    {
        Vector3 startPosition = _cameraParent.localPosition;
        Quaternion startRotation = _cameraParent.localRotation;

        Vector3 targetPosition;
        Quaternion targetRotation;

        if (opening)
        {
            targetPosition = _cameraPositionBeforeInventory + _inventoryPositionOffset;

            targetRotation = _cameraRotationBeforeInventory * Quaternion.Euler(_inventoryRotationOffset);
        }
        else
        {
            targetPosition = _cameraPositionBeforeInventory;
            targetRotation = _cameraRotationBeforeInventory;
        }

        float elapsed = 0f;

        while (elapsed < _cameraTransitionDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / _cameraTransitionDuration);

            float curvedT = _cameraCurve.Evaluate(t);

            _cameraParent.localPosition = Vector3.Lerp(startPosition, targetPosition, curvedT);

            _cameraParent.localRotation = Quaternion.Slerp(startRotation, targetRotation, curvedT);

            if (opening && _cameraLookController != null)
            {
                _cameraLookController.SetInventoryCameraBlend(curvedT);
            }

            float fade = opening ? Mathf.Lerp(0f, 1f, curvedT) : Mathf.Lerp(1f, 0f, curvedT);

            _fadeController.SetFade(fade);

            yield return null;
        }

        _cameraParent.localPosition = targetPosition;
        _cameraParent.localRotation = targetRotation;

        _fadeController.SetFade(opening ? 1f : 0f);
    }
    

}