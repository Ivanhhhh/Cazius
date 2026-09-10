using System.Collections;
using System.Net.Sockets;
using TMPro;
using UnityEngine;

public class UIInteraction : MonoBehaviour
{

    [SerializeField] private GameObject _containerUI;
    [SerializeField] private TextMeshPro _interactText;
    [SerializeField] private MeshRenderer _backgroundRenderer;

    [SerializeField] private Material _alwaysOnTopTextMaterial;
    [SerializeField] private Material _lockMaterial;

    private bool _currentLockedInteractableState = false;

    [SerializeField] private float _fadeDuration = 0.2f;

    [SerializeField] private float _hiddenVertexOffset = 0.2f;
    [SerializeField] private float _visibleVertexOffset = 0f;

    private bool _dialogOpen;

    private IEInteractable _currentInteractable;
    private Transform _currentTarget;

    private Camera _camera;

    private Material _backgroundMaterial;
    private Coroutine _transitionCoroutine;

    private bool _shouldBeVisible;

    private bool _isLockShown = false;

    private void Awake()
    {
        _backgroundMaterial = _backgroundRenderer.material;
        _interactText.fontSharedMaterial = _alwaysOnTopTextMaterial;

        SetHiddenImmediate();
    }

    private void OnEnable()
    {
        _lockMaterial.SetFloat("_AlphaMult", 0f);
        PlayerInteract.OnInteractableChanged += OnInteractableChanged;

        DialogUIController.OnDialogOpened += OnDialogOpened;
        DialogUIController.OnDialogClosed += OnDialogClosed;
    }

    private void OnDisable()
    {
        PlayerInteract.OnInteractableChanged -= OnInteractableChanged;

        DialogUIController.OnDialogOpened -= OnDialogOpened;
        DialogUIController.OnDialogClosed -= OnDialogClosed;
    }

    private void LateUpdate()
    {
        if (_currentTarget == null)
            return;

        UpdatePrompt();
    }

    private void OnInteractableChanged(IEInteractable interactable)
    {
        if (_camera == null)
        {
            _camera = Camera.main;
        }

        _currentInteractable = interactable;

        if (_dialogOpen)
            return;

        if (interactable != null)
            Show(interactable);
        else
            Hide();
    }

    private void Show(IEInteractable interactable)
    {
        _currentInteractable = interactable;
        _currentTarget = interactable.GetInteractionUIPoint();

        _interactText.text = IsInteractableLocked(interactable.IsLocked());

        if (_alwaysOnTopTextMaterial != null && _interactText.fontSharedMaterial != _alwaysOnTopTextMaterial)
        {
            _interactText.fontSharedMaterial = _alwaysOnTopTextMaterial;
        }

        UpdatePrompt();

        _shouldBeVisible = true;
        StartTransition(true);

    }

    private void Hide()
    {
        _shouldBeVisible = false;
        StartTransition(false);
    }

    [SerializeField] private Vector3 _rotationOffset = new Vector3(-90f, 0f, 0f);

    private void UpdatePrompt()
    {
        if (_currentTarget == null || _camera == null)
            return;


            _containerUI.transform.position = _currentTarget.position;

        Vector3 direction = _camera.transform.position - _containerUI.transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction, _camera.transform.up);

        _containerUI.transform.rotation = lookRotation * Quaternion.Euler(_rotationOffset);
    }

    private string IsInteractableLocked(bool locked)
    {
        _currentLockedInteractableState = locked;

        if (locked == true)
        {
            _isLockShown = true;
            return "Locked";
                //_lockImage.SetActive(true);
        }
        else
        {
            _isLockShown = false;
            return "F";
                //_lockImage.SetActive(false);
        }
    }

    private void StartTransition(bool show)
    {
        if (_transitionCoroutine != null)
            StopCoroutine(_transitionCoroutine);

        _transitionCoroutine = StartCoroutine(TransitionPrompt(show));
    }

    private IEnumerator TransitionPrompt(bool show)
    {

        float startOpacity = _backgroundMaterial.GetFloat("_OpacityMultiplier");

        float startVertexOffset = _backgroundMaterial.GetFloat("_VertexOffset");

        float startTextAlpha = _interactText.alpha;

        float lockStartOpacity = _lockMaterial.GetFloat("_AlphaMult");


        float targetOpacity = show ? 1f : 0f;

        float targetVertexOffset = show ? _visibleVertexOffset : _hiddenVertexOffset;

        float targetTextAlpha = show ? 1f : 0f;

        if (!_isLockShown)
        {
            _lockMaterial.SetFloat("_AlphaMult", 0f);
        }
        else
        {
            _interactText.alpha = 0f;
        }


            float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / _fadeDuration);

            t = Mathf.SmoothStep(0f, 1f, t);


            float opacity = Mathf.Lerp(startOpacity, targetOpacity, t);

            float vertexOffset = Mathf.Lerp(startVertexOffset, targetVertexOffset, t);

            float textAlpha = Mathf.Lerp(startTextAlpha, targetTextAlpha, t);


            _backgroundMaterial.SetFloat("_OpacityMultiplier", opacity);

            if (_isLockShown)
            {
                _lockMaterial.SetFloat("_AlphaMult", opacity);
            }
            else
            {
                _interactText.alpha = textAlpha;
            }

            _backgroundMaterial.SetFloat("_VertexOffset", vertexOffset);


            yield return null;
        }

        _backgroundMaterial.SetFloat("_OpacityMultiplier", targetOpacity);

        _backgroundMaterial.SetFloat("_VertexOffset", targetVertexOffset);

        if (!_isLockShown) { _interactText.alpha = targetTextAlpha; }

        if (!show && !_shouldBeVisible)
        {
            _currentTarget = null;
        }

        _transitionCoroutine = null;
    }

    private void SetHiddenImmediate()
    {
        _backgroundMaterial.SetFloat("_OpacityMultiplier", 0f);

        _backgroundMaterial.SetFloat("_VertexOffset", _hiddenVertexOffset);

        _interactText.alpha = 0f;
    }


    private void OnDialogOpened()
    {
        _dialogOpen = true;
        Hide();
    }

    private void OnDialogClosed()
    {
        _dialogOpen = false;
        if (_currentInteractable != null)
        {
            Show(_currentInteractable);
        }
    }
}