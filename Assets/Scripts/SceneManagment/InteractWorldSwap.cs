
using System.Collections;
using UnityEngine;

public class InteractWorldSwap : MonoBehaviour, IEInteractable
{
    [Header("Interact")]
    [SerializeField] private string _interactText = "F to Swap World";
    [SerializeField] private float _cooldown = 4f;

    [Header("Scenes")]
    [SerializeField] private SceneField[] _scenesToLoadPurgatory;
    [SerializeField] private SceneField[] _scenesToLoadEden;

    [Header("Proximity Animation")]
    [SerializeField] private Transform _player;
    [SerializeField] private float _nearDistance = 3f;

    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private string _nearBoolName = "Near";
    [SerializeField] private string _activateTriggerName = "Activate";
    [SerializeField] private float _activationAnimDuration = 1.5f;

    [Header("Rune Materials")]
    [SerializeField] private Renderer[] _runeRenderers = new Renderer[3];
    [SerializeField] private string _runeActiveProperty = "_RuneActive";

    private bool _canInteract = true;
    private bool _isActivating;

    private Material[] _runeMaterialInstances;

    [SerializeField] private Transform _interactionUIPoint;

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    private void Awake()
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        // Create material instances for all rune renderers
        _runeMaterialInstances = new Material[_runeRenderers.Length];

        for (int i = 0; i < _runeRenderers.Length; i++)
        {
            if (_runeRenderers[i] == null)
                continue;

            _runeMaterialInstances[i] = _runeRenderers[i].material;
        }

        SetRuneActive(false);
    }

    private void Start()
    {
        if (_player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
                _player = playerObject.transform;
        }
    }

    private void Update()
    {
        HandleNearAnimation();
    }

    public void Interact(Transform interactorTransform)
    {
        if (!_canInteract)
            return;

        if (WorldChangeManager.Instance == null)
        {
            Debug.LogWarning("WorldChangeManager Instance is missing.");
            return;
        }

        StartCoroutine(SwapRoutine());
    }

    private IEnumerator SwapRoutine()
    {
        _canInteract = false;
        _isActivating = true;

        SetRuneActive(true);

        if (_animator != null)
            _animator.SetTrigger(_activateTriggerName);

        if (WorldChangeManager.Instance.IsInEden)
        {
            WorldChangeManager.Instance.SwapToPurgatory(
                _scenesToLoadPurgatory);
        }
        else
        {
            WorldChangeManager.Instance.SwapToEden(
                _scenesToLoadEden);
        }

        yield return new WaitForSeconds(_activationAnimDuration);

        _isActivating = false;
        SetRuneActive(false);

        yield return new WaitForSeconds(
            Mathf.Max(0f, _cooldown - _activationAnimDuration));

        _canInteract = true;
    }

    private void HandleNearAnimation()
    {
        if (_player == null)
            return;

        if (_isActivating)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, _player.position);

        bool isNear = distanceToPlayer <= _nearDistance;

        SetRuneActive(isNear);

        if (_animator != null)
            _animator.SetBool(_nearBoolName, isNear);
    }

    private void SetRuneActive(bool active)
    {
        float value = active ? 1f : 0f;

        foreach (Material material in _runeMaterialInstances)
        {
            if (material == null)
                continue;

            material.SetFloat(_runeActiveProperty, value);
        }
    }

    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsLocked()
    {
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _nearDistance);
    }
}
