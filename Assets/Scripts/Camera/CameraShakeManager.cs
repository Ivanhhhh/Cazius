using System.Collections;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance { get; private set; }

    [SerializeField] private Transform _shakePivot;

    private Vector3 _originalLocalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_shakePivot != null)
        {
            _originalLocalPosition = _shakePivot.localPosition;
        }
    }

    public void CameraShake(float shakeStrength, float shakeDuration)
    {
        if (_shakePivot == null)
            return;

        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
            _shakeCoroutine = null;
        }

        _shakePivot.localPosition = _originalLocalPosition;

        Debug.Log("ShakeCalled");

        _shakeCoroutine = StartCoroutine(CameraShakeCoroutine(shakeStrength, shakeDuration));
    }

    private IEnumerator CameraShakeCoroutine(float shakeStrength, float shakeDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < shakeDuration)
        {
            Vector2 randomOffset = Random.insideUnitCircle * shakeStrength;

            _shakePivot.localPosition = _originalLocalPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _shakePivot.localPosition = _originalLocalPosition;

        _shakeCoroutine = null;
    }

    private void OnDisable()
    {
        if (_shakePivot != null)
        {
            _shakePivot.localPosition = _originalLocalPosition;
        }
    }
}