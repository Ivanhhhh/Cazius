using System.Collections;
using UnityEngine;

public class SkySunFromDirectionalLight : MonoBehaviour
{
    [SerializeField] private Camera targetCamera;
    [SerializeField] private Light sunLight;

    [SerializeField] private float distance = 500f;

    [SerializeField] private bool startVisibleInEden = true;

    private Renderer[] _renderers;
    private bool _isVisible;

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>(true);
    }

    private void Start()
    {
        StartCoroutine(FindCameraRoutine());
        StartCoroutine(SubscribeToWorldChangeManager());

        SetSunVisible(startVisibleInEden);
    }
    private IEnumerator FindCameraRoutine()
    {
        while (targetCamera == null)
        {
            targetCamera = Camera.main;
            yield return null;
        }
    }

    private IEnumerator SubscribeToWorldChangeManager()
    {
        while (WorldChangeManager.Instance == null)
        {
            yield return null;
        }

        WorldChangeManager.Instance.SwapToEdenEvent += ShowSun;
        WorldChangeManager.Instance.SwapToPurgatoryEvent += HideSun;
    }

    private void OnDestroy()
    {
        if (WorldChangeManager.Instance == null)
            return;

        WorldChangeManager.Instance.SwapToEdenEvent -= ShowSun;
        WorldChangeManager.Instance.SwapToPurgatoryEvent -= HideSun;
    }

    private void LateUpdate()
    {
        if (!_isVisible)
            return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null || sunLight == null)
            return;

        Vector3 sunDirection = -sunLight.transform.forward;

        transform.position = targetCamera.transform.position + sunDirection * distance;

        transform.forward = transform.position - targetCamera.transform.position;
    }

    private void ShowSun()
    {
        SetSunVisible(true);
    }

    private void HideSun()
    {
        SetSunVisible(false);
    }

    private void SetSunVisible(bool visible)
    {
        _isVisible = visible;

        foreach (Renderer renderer in _renderers)
        {
            renderer.enabled = visible;
        }
    }
}