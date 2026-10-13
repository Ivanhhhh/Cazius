using System.Collections;
using UnityEngine;

public class DitheredTransparency : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer;

    private Material _material;

    private static readonly int BaseColorID = Shader.PropertyToID("_Base_Color");

    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponent<SkinnedMeshRenderer>();

        _material = _renderer.material;
    }

    public void FadeAlphaToZero(float fadeDuration)
    {
        StartCoroutine(FadeCoroutine(fadeDuration));
    }

    private IEnumerator FadeCoroutine(float coroutineeFadeDuration)
    {
        float timer = 0f;

        Color startColor = _material.GetColor(BaseColorID);
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        while (timer < coroutineeFadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / coroutineeFadeDuration;
            Color newColor = Color.Lerp(startColor, endColor, t);

            _material.SetColor(BaseColorID, newColor);

            yield return null;
        }

        _material.SetColor(BaseColorID, endColor);
    }
}
