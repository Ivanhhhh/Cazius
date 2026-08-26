using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player_Damage : MonoBehaviour
{
    [SerializeField] private Volume _volume;

    private Vignette _vignette;
    private Coroutine _damageCoroutine;

    private void Start()
    {
        if (_volume == null)
        {
            Debug.LogError("Global Volume empty");
            return;
        }

        if (!_volume.profile.TryGet(out _vignette))
        {
            Debug.LogError("Viggnete empty");
            return;
        }

        _vignette.intensity.Override(0f);
    }

    public void TakeDamageEffect()
    {
        if (_damageCoroutine != null)
            StopCoroutine(_damageCoroutine);

        _damageCoroutine = StartCoroutine(DamageEffect());
    }

    private IEnumerator DamageEffect()
    {
        float intensity = 0.47f;

        _vignette.intensity.Override(intensity);

        yield return new WaitForSeconds(0.47f);

        while (intensity > 0f)
        {
            intensity -= 0.01f;

            if (intensity < 0f)
                intensity = 0f;

            _vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }

        _damageCoroutine = null;
    }
}
