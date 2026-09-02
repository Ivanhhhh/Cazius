using System.Collections;
using TMPro;
using UnityEngine;

public class TextWorldScan : MonoBehaviour
{
    [SerializeField] TMP_Text[] _mesh;

    private Camera _camera;
    private bool _subscribed;

    public bool lookAtCam = true;

    private void Awake()
    {
        StartCoroutine(GetCamWhenReady());
        DisableObject();
    }

    private IEnumerator GetCamWhenReady()
    {

        yield return new WaitForSeconds(10f);
       
            _camera = Camera.main;

            if (_camera == null)
            {
                _camera = FindFirstObjectByType<Camera>();
            }

            yield return null;
    }

    private void OnEnable()
    {
        StartCoroutine(SubscribeWhenReady());
    }

    private IEnumerator SubscribeWhenReady()
    {
        while (WorldScanManager.Instance == null)
        {
            yield return null;
        }

        WorldScanManager.Instance.ScanActive += EnableObject;
        WorldScanManager.Instance.ScanDeactivate += DisableObject;

        _subscribed = true;
    }

    private void OnDisable()
    {
        if (!_subscribed)
            return;

        if (WorldScanManager.Instance != null)
        {
            WorldScanManager.Instance.ScanActive -= EnableObject;
            WorldScanManager.Instance.ScanDeactivate -= DisableObject;
        }

        _subscribed = false;
    }

    private void EnableObject()
    {
        foreach (var obj in _mesh)
        {
            StartCoroutine(FadeText(0f, 0.8f, 1f, obj));
        }
    }

    private void DisableObject()
    {
        foreach (var obj in _mesh)
        {
            StartCoroutine(FadeText(0.8f, 0f, 1f, obj));
        }
    }

    private void LateUpdate()
    {
        if (_camera == null || lookAtCam == false)
            return;

        Vector3 directionToCamera =
            _camera.transform.position - transform.position;

        if (directionToCamera.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }

    private IEnumerator FadeText(float start, float end, float duration, TMP_Text text)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {

            elapsed += Time.unscaledDeltaTime;

            float value = Mathf.Lerp(start, end, elapsed / duration);

                text.alpha = value;

            yield return null;
        }


            text.alpha = end;

        yield return null;
    }
}
