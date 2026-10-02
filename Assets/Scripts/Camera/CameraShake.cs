using UnityEngine;
using SmoothShakeFree;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("Smooth Shake Free")]
    [SerializeField] private SmoothShake shake;
    [SerializeField] private SmoothShakeFreePreset preset;

    [Header("Collapse Shake")]
    [SerializeField] private float shakeSmoothness = 4f;
    [SerializeField] private float returnDuration = 0.5f;

    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private Coroutine shakeCoroutine;

    private void Awake()
    {
        Instance = this;

        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
    }

    public void DamageShake()
    {
        shake.StartShake(preset);

        Debug.Log("Damage Shake ON");
    }

    public void StartShake(float strength)
    {
        if (shakeCoroutine != null)
            return;

        shakeCoroutine = StartCoroutine(ShakeCoroutine(strength));

        Debug.Log("Collapse Shake ON");
    }

    public void StopShake()
    {
        if (shakeCoroutine == null)
            return;

        StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ReturnToOriginalPosition());

        Debug.Log("Collapse Shake OFF");
    }

    private IEnumerator ShakeCoroutine(float strength)
    {
        while (true)
        {
            float x = (Mathf.PerlinNoise(Time.time * shakeSmoothness, 0f) - 0.5f) * 2f;
            float y = (Mathf.PerlinNoise(0f, Time.time * shakeSmoothness) - 0.5f) * 2f;

            Vector3 offset = new Vector3(x, y, 0f) * strength;

            transform.localPosition = originalLocalPosition + offset;

            yield return null;
        }
    }

    private IEnumerator ReturnToOriginalPosition()
    {
        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;

        float elapsed = 0f;

        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / returnDuration);
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localPosition = Vector3.Lerp(
                startPosition,
                originalLocalPosition,
                t
            );

            transform.localRotation = Quaternion.Slerp(
                startRotation,
                originalLocalRotation,
                t
            );

            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        transform.localRotation = originalLocalRotation;

        shakeCoroutine = null;
    }
}