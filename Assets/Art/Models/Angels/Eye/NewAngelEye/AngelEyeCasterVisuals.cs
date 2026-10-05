using UnityEngine;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;

public class AngelEyeCasterVisuals : MonoBehaviour
{
    [SerializeField] MeshRenderer _eyeRenderer;
    [SerializeField] MeshRenderer[] _ringRenderers;
    [SerializeField] SkinnedMeshRenderer _skinnedRingRenderers;
    [SerializeField] Animator _animator;
    [SerializeField] GameObject _nonDissolveMat;

    private Material _eyeMat;
    private Material[] _ringMats;
    private Material _skinnedMat;


    [SerializeField] float _eyeDamageDuration = 0.5f;
    [SerializeField] float _phaseChangeDelay = 1f;
    [SerializeField] float _dissolveDuration = 1f;


    private void Awake()
    {
        _eyeMat = _eyeRenderer.material;

        _skinnedMat = _skinnedRingRenderers.material;

        _ringMats = new Material[_ringRenderers.Length];

        for (int i = 0; i < _ringRenderers.Length; i++)
            _ringMats[i] = _ringRenderers[i].material;
    }

    public void EyeTakeDMG()
    {
        _animator.SetTrigger("TakeDamage");

        StartCoroutine(EyeDamageCoroutine());
    }

    public void EyeDie()
    {
        _animator.SetTrigger("Die");

        StartCoroutine(DieCourutine());
    }
    private IEnumerator EyeDamageCoroutine()
    {
        float halfDuration = _eyeDamageDuration / 2f;

        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(0f, 1f, t / halfDuration);
            _eyeMat.SetFloat("_EmissiveAmount", value);

            yield return null;
        }

        _eyeMat.SetFloat("_EmissiveAmount", 1f);

        for (float t = 0f; t < halfDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(1f, 0f, t / halfDuration);
            _eyeMat.SetFloat("_EmissiveAmount", value);

            yield return null;
        }

        _eyeMat.SetFloat("_EmissiveAmount", 0f);
    }

    private IEnumerator DieCourutine()
    {
        yield return new WaitForSeconds(_phaseChangeDelay);

        _nonDissolveMat.SetActive(false);

        for (float t = 0f; t < _dissolveDuration; t += Time.deltaTime)
        {
            float value = Mathf.Lerp(0f, 1f, t / _dissolveDuration);

            _skinnedMat.SetFloat("_DissolveAmount", value);

            foreach (var mat in _ringMats)
                mat.SetFloat("_DissolveAmount", value);

            yield return null;
        }

        foreach (var mat in _ringMats)
            mat.SetFloat("_DissolveAmount", 1f);
    }

}
