using System.Collections;
using UnityEngine;

public class BuildingFallDestroy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator _animator;

    [Header("Initial Shake")]
    [SerializeField] private float _initialShakeStrength = 0.05f;
    [SerializeField] private float _initialShakeDuration = 2f;

    [Header("Impact Shake")]
    [SerializeField] private float _impactShakeStrength = 0.3f;
    [SerializeField] private float _impactShakeDuration = 2f;
    [SerializeField] private float _impactShakeDelay = 1.5f;

    private bool _hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (_hasTriggered)
            return;

        _hasTriggered = true;

        StartCoroutine(BuildingDestroyCoroutine());
    }

    private IEnumerator BuildingDestroyCoroutine()
    {
        // Start building destruction animation
        _animator.SetTrigger("Destroy");

        // Small initial camera shake
        CameraShakeManager.Instance.CameraShake(
            _initialShakeStrength,
            _initialShakeDuration
        );

        // Wait until building impacts the ground
        yield return new WaitForSeconds(_impactShakeDelay);

        // Strong impact camera shake (overwrites previous shake)
        CameraShakeManager.Instance.CameraShake(
            _impactShakeStrength,
            _impactShakeDuration
        );

        // FUTURE: Activate destruction particles here
    }
}