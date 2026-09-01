using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldScanManager : MonoBehaviour
{

    public static WorldScanManager Instance;

    public event Action ScanActive;
    public event Action ScanDeactivate;

    [SerializeField] private List<SpheresScan> _spheres = new List<SpheresScan>();
    [SerializeField] private float _spheresMaxScale;
    [SerializeField] private float _spheresMinScale;
    [SerializeField] private float _spheresScaleOffset;
    [SerializeField] private float _spheresTransitionDuration;
    [SerializeField] private float _timeBetweenSphere;

    [SerializeField] private Material _scanLinesFCShader;
    [SerializeField] private float _scanLinesDistance;
    [SerializeField] private float _scanLinesTransitionDuration;


    private bool _scanActive = false;

    private bool _inventoryActive;
    private bool _scanTransitioning;

    private Coroutine _scanCoroutine;

    public bool IsScanActive => _scanActive;

    public PlayerControls _controls;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;


        _controls = GameInputManager.Instance.Controls;
        _scanLinesFCShader.SetFloat("_LinesEndFade", 0);
    }

    private void OnEnable()
    {
        _controls.Player.Scan.performed += OnScanPerformed;
    }

    private void OnDisable()
    {
        _controls.Player.Scan.performed -= OnScanPerformed;
    }

    public void SetInventoryActive(bool active)
    {
        _inventoryActive = active;

        if (active)
        {
            ForceDeactivateScan();
        }
    }

    public void ForceDeactivateScan()
    {
        if (_scanCoroutine != null)
        {
            StopCoroutine(_scanCoroutine);
            _scanCoroutine = null;
        }

        if (_scanActive) 
        _scanCoroutine = StartCoroutine(DeactivateScan(_spheres));
    }

    private void OnScanPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (_inventoryActive)
            return;

        if (_scanTransitioning)
            return;

        DoScan();
    }


    private void DoScan()
    {
        if (_scanCoroutine != null)
            StopCoroutine(_scanCoroutine);

        if (!_scanActive)
        {
            _scanCoroutine = StartCoroutine(
                ActivateScan(_spheres)
            );
        }
        else
        {
            _scanCoroutine = StartCoroutine(
                DeactivateScan(_spheres)
            );
        }
    }

    private IEnumerator ActivateScan(List<SpheresScan> spheres)
    {
        _scanTransitioning = true;
        _scanActive = true;

        StartCoroutine(FadeShader(_scanLinesFCShader, "_LinesEndFade", 0f, _scanLinesDistance, _scanLinesTransitionDuration));

        ScanActive?.Invoke();

        float spheresScale = _spheresMaxScale;

        foreach(SpheresScan ball in spheres)
        {
            ball.Grow(_spheresTransitionDuration, spheresScale);

            spheresScale -= _spheresScaleOffset;

            yield return new WaitForSecondsRealtime(_timeBetweenSphere);
        }

        _scanTransitioning = false;
        _scanCoroutine = null;
    }

    private IEnumerator DeactivateScan(List<SpheresScan> spheres)
    {
        _scanTransitioning = true;
        _scanActive = false;

        StartCoroutine(FadeShader(_scanLinesFCShader, "_LinesEndFade", _scanLinesDistance, 0f, _scanLinesTransitionDuration));

        ScanDeactivate?.Invoke();

        for (int i = spheres.Count - 1; i >= 0; i--)
        {
            spheres[i].Contract(_spheresTransitionDuration, _spheresMinScale);

            yield return new WaitForSecondsRealtime(_timeBetweenSphere);
        }

        _scanTransitioning = false;
        _scanCoroutine = null;
    }

    private IEnumerator FadeShader(Material mat, string property, float start, float end, float duration)
    {
        float elapsed = 0f;
        mat.SetFloat(property, start);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float value = Mathf.Lerp(start, end, elapsed / duration);
            mat.SetFloat(property, value);
            yield return null;
        }

        mat.SetFloat(property, end);
    }

}
