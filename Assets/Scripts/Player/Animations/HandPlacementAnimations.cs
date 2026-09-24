using UnityEngine;
using System.Collections;

public class HandPlacementAnimations : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [Header("Collision")]
    [SerializeField] LayerMask _collisionLayer;

    [Header("Muzzle Detection")]
    [SerializeField] Transform _muzzle;
    [SerializeField] float _muzzleRadius = 0.15f;

    [Header("Clearance Detection")]
    [SerializeField] Transform _clearanceOrigin;
    [SerializeField] Transform _aimDirection;
    [SerializeField] float _clearanceDistance = 0.8f;

    [Header("Return To Aim")]
    [SerializeField] float _clearDelay = 0.1f;

    [Header("Animation Blend")]
    [SerializeField] float _gunBlendDuration = 0.2f;

    private Coroutine _gunBlendCoroutine;

    private bool _weaponBlocked;
    private float _clearTimer;

    private void Update()
    {
        if (!GameManager.Instance.gunActive)
        {  return; }

        if (!_weaponBlocked)
        {
            CheckMuzzleCollision();
        }
        else
        {
            CheckClearance();
        }
    }

    private void CheckMuzzleCollision()
    {
        bool muzzleBlocked = Physics.CheckSphere(_muzzle.position, _muzzleRadius, _collisionLayer, QueryTriggerInteraction.Ignore);

        if (muzzleBlocked)
        {
            SetWeaponBlocked(true);
        }
    }

    private void CheckClearance()
    {
        bool blocked = Physics.Raycast(_clearanceOrigin.position, _aimDirection.forward, _clearanceDistance, _collisionLayer, QueryTriggerInteraction.Ignore);

        if (!blocked)
        {
            _clearTimer += Time.deltaTime;

            if (_clearTimer >= _clearDelay)
            {
                SetWeaponBlocked(false);
            }
        }
        else
        {
            _clearTimer = 0f;
        }
    }

    private void SetWeaponBlocked(bool blocked)
    {
        _weaponBlocked = blocked;
        _clearTimer = 0f;

        float targetValue = blocked ? 0f : 1f;

        if (_gunBlendCoroutine != null)
        {
            StopCoroutine(_gunBlendCoroutine);
        }

        _gunBlendCoroutine = StartCoroutine(BlendHasGun(targetValue));
    }

    private void OnDrawGizmosSelected()
    {
        if (_muzzle != null)
        {
            Gizmos.DrawWireSphere(_muzzle.position, _muzzleRadius);
        }

        if (_clearanceOrigin != null && _aimDirection != null)
        {
            Gizmos.DrawRay(_clearanceOrigin.position, _aimDirection.forward * _clearanceDistance);
        }
    }

    private IEnumerator BlendHasGun(float targetValue)
    {
        float startValue = _animator.GetFloat("HasGun");
        float elapsedTime = 0f;

        while (elapsedTime < _gunBlendDuration)
        {
            elapsedTime += Time.deltaTime;

            float blend = elapsedTime / _gunBlendDuration;
            float newValue = Mathf.Lerp(startValue, targetValue, blend);

            _animator.SetFloat("HasGun", newValue);

            yield return null;
        }

        _animator.SetFloat("HasGun", targetValue);

        _gunBlendCoroutine = null;
    }
}
