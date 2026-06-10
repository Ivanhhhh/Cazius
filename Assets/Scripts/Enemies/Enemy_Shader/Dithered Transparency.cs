using System.Collections;
using UnityEngine;

public class DitheredTransparency : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private float _fadeDuration = 1.5f;

    private Material _material;

    private static readonly int BaseColorID = Shader.PropertyToID("_Base_Color");

    private void Awake()
    {
        if (_renderer == null) _renderer = GetComponent<SkinnedMeshRenderer>();

        _material = _renderer.material;
    }

    public void FadeAlphaToZero()
    {
        StartCoroutine(FadeCoroutine());
    }

    private IEnumerator FadeCoroutine()
    {
        float timer = 0f;

        Color startColor = _material.GetColor(BaseColorID);
        startColor.a = 1f;

        Color endColor = startColor;
        endColor.a = 0f;

        while (timer < _fadeDuration)
        {
            timer += Time.deltaTime;

            float t = timer / _fadeDuration;
            Color newColor = Color.Lerp(startColor, endColor, t);

            _material.SetColor(BaseColorID, newColor);

            yield return null;
        }

        _material.SetColor(BaseColorID, endColor);
    }
}
