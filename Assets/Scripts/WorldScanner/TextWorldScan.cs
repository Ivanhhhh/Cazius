using UnityEngine;
using System.Collections;

public class TextWorldScan : MonoBehaviour
{
    [SerializeField] MeshRenderer[] _mesh;

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
            obj.enabled = true;
        }
    }

    private void DisableObject()
    {
        foreach (var obj in _mesh)
        {
            obj.enabled = false;
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
}
