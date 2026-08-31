using UnityEngine;
using System.Collections;
using System;

public class DiageticHealthBarUIManager : MonoBehaviour
{
    [SerializeField] MeshRenderer _material;

    [SerializeField] float _fadeDuration;

    [SerializeField] float _hiddenVertexOffset = 0.5f;
    [SerializeField] float _visibleVertexOffset = 0.0f;

    [SerializeField] float _takeDamageFadeDur = 0.2f;
    [SerializeField] float _takeDamageVertexOffset = 0.3f;
    private float _oldPercentage = 1.0f;


    private bool _subscribed;
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
        StartCoroutine(FadeShader(_material, "_OpacityMult", 0f, 1f, _fadeDuration));
        StartCoroutine(FadeShader(_material, "_VertexOffset", _hiddenVertexOffset, _visibleVertexOffset, _fadeDuration));
    }

    private void DisableObject()
    {
        StartCoroutine(FadeShader(_material, "_OpacityMult", 1f, 0f, _fadeDuration));
        StartCoroutine(FadeShader(_material, "_VertexOffset", _visibleVertexOffset, _hiddenVertexOffset, _fadeDuration));
    }

    public void ChangeHealthBarPercentage(float percentage)
    {
       StartCoroutine(ChangeHealthVisuals(percentage));
    }

    private IEnumerator ChangeHealthVisuals(float percentage)
    {
        StartCoroutine(FadeShader(_material, "_HealthPrecentage", _oldPercentage, percentage, _takeDamageFadeDur));

        _oldPercentage = percentage;

        yield return StartCoroutine(FadeShader(_material, "_VertexOffset", 0, _takeDamageVertexOffset, _takeDamageFadeDur / 2));
        yield return StartCoroutine(FadeShader(_material, "_VertexOffset", _takeDamageVertexOffset, 0, _takeDamageFadeDur / 2));
    }

    private IEnumerator FadeShader(MeshRenderer mat, string property, float start, float end, float duration)
    {
        float elapsed = 0f;
        mat.material.SetFloat(property, start);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float value = Mathf.Lerp(start, end, elapsed / duration);
            mat.material.SetFloat(property, value);
            yield return null;
        }

        mat.material.SetFloat(property, end);
    }
}
