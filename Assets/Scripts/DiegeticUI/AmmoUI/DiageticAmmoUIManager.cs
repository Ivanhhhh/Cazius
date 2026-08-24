using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class DiageticAmmoUIManager : MonoBehaviour
{

    [SerializeField] MeshRenderer _backgroundMat;
    [SerializeField] TMP_Text textComponent;

    [SerializeField] float _fadeDuration = 1.0f;

    private Coroutine _fadeCoroutine;

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
        StartCoroutine(FadeShader(_backgroundMat, "_OpacityMultiplier", 0f, 1f, _fadeDuration));
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", 0.2f, 0f, _fadeDuration));
        StartCoroutine(FadeText(0f, 1f, _fadeDuration));
    }

    private void DisableObject()
    {
        StartCoroutine(FadeShader(_backgroundMat, "_OpacityMultiplier", 1f, 0f, _fadeDuration));
        StartCoroutine(FadeShader(_backgroundMat, "_VertexOffset", 0f, 0.2f, _fadeDuration));
        StartCoroutine(FadeText(1f, 0f, _fadeDuration));
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

    private IEnumerator FadeText(float start, float end, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float value = Mathf.Lerp(start,end, elapsed / duration);
            textComponent.alpha = value;

            yield return null;
        }

        textComponent.alpha = end;

        yield return null;
    }

}
