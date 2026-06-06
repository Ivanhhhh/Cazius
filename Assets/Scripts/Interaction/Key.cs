using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Key : MonoBehaviour, IEInteractable
{
    [Header("Key type")]
    [SerializeField] private bool _isEdenKey;
    [SerializeField] private bool _isPurgatoryKey;

    [Header("Key animation")]
    [SerializeField] private Transform _keyRoot;
    [SerializeField] private Animator _keyAnimator;
    [SerializeField] private GameObject _currentCamera;
    [SerializeField] private GameObject _secondCamera;
    [SerializeField] private Vector3 _offsetKey;

    [Header("UI")]
    [SerializeField] private GameObject _keyPanel;
    [SerializeField] private string _interactText = "F to Grab Key";

    [SerializeField] private ObjectsActivator _objectsToActivate;
    void Awake()
    {
        _keyAnimator.enabled = false;
    }
    public void Interact(Transform interactorTransform)
    {
        if (_isPurgatoryKey)
        {
            KeyInventorySystem.Instance.AddPurgatoryKey();
        }

        if (_isEdenKey)
        {
            KeyInventorySystem.Instance.AddEdenKey();
            if (_objectsToActivate != null)
            {
                _objectsToActivate.Activate();
            }
        }

        _keyPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public IEnumerator WhenKeyOpenDoor(Transform doorTransform, string animationName)
    {
        gameObject.SetActive(true);
        _currentCamera.SetActive(false);

        _keyAnimator.enabled = false;

        _keyRoot.position = doorTransform.position + _offsetKey;

        _secondCamera.SetActive(true);
        // _secondCamera.transform.position = _posSecondCamera;
        // _secondCamera.transform.rotation = _rotationSecondCamera;

        _keyAnimator.enabled = true;
        _keyAnimator.Play(animationName, 0, 0f);

        yield return new WaitForSeconds(3f);

        _keyAnimator.enabled = false;
        gameObject.SetActive(false);
        _secondCamera.SetActive(false);
        _currentCamera.SetActive(true);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}
