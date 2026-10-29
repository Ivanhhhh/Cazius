using UnityEngine;

public class ShootInteraction : MonoBehaviour, IShootable
{
    [Header("UsesAnimator")]
    [SerializeField] Animator _animator;
    [SerializeField] string _animTriggerName;

    [Header("EnablesObject")]
    [SerializeField] GameObject _objToEnable;
    [SerializeField] Transform _objSpawnPos;

    [Header("DisablesObject")]
    [SerializeField] GameObject _objToDisable;
    [SerializeField] Collider _shootCollider;

    [Header("IfImpactParticle")]
    [SerializeField] GameObject _impactParticle;

    [Header("Glowparticle")]
    [SerializeField] GameObject _glowParticle;

    private void Awake()
    {
        if (_animator != null)
        {
        _animator.keepAnimatorStateOnDisable = true;
        _animator.writeDefaultValuesOnDisable = false;
        }

        if (_impactParticle != null)
        {
            _impactParticle.SetActive(false);
        }
    }

    public void TakeDamage(float amount)
    {
        if (_animator != null && _animTriggerName != null) { _animator.SetTrigger(_animTriggerName); }

        if(_objToEnable != null)
        {
            _objToEnable.SetActive(true);

            if (_objSpawnPos != null) { _objToEnable.transform.position = _objSpawnPos.transform.position;}
        }

        if (_objToDisable != null) { _objToDisable.SetActive(false); }

        if (_shootCollider != null) { _shootCollider.enabled = false; }

        if (_glowParticle != null) { _glowParticle.SetActive(false); }

        if (_impactParticle != null)
        {
            _impactParticle.SetActive(true);
            if (_impactParticle.GetComponent<ParticleSystem>() != null) { _impactParticle.GetComponent<ParticleSystem>().Play(); }
        }
    }
}
