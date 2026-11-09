
using UnityEngine;
using System.Collections;

public class HandPlacementAnimations : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [Header("Collision")]
    [SerializeField] LayerMask _collisionLayer;

    [Header("Clearance Detection")]
    [SerializeField] Transform _clearanceOrigin;
    [SerializeField] Transform _aimDirection;
    [SerializeField] float _blockDistance = 0.8f;
    [SerializeField] float _clearDistance = 1f;
    [SerializeField] float _collisionRadius = 0.15f;

    [Header("Detection Delay")]
    [SerializeField] float _blockDelay = 0.05f;
    [SerializeField] float _clearDelay = 0.2f;

    [Header("Animation Blend")]
    [SerializeField] float _gunBlendDuration = 0.2f;

    private Coroutine _gunBlendCoroutine;
    private bool _weaponBlocked;
    private float _blockTimer;
    private float _clearTimer;

    private void Update()
    {
        if (!GameManager.Instance.gunActive)
            return;

        float distance = _weaponBlocked ? _clearDistance : _blockDistance;

        bool originBlocked = Physics.CheckSphere(
            _clearanceOrigin.position,
            _collisionRadius,
            _collisionLayer,
            QueryTriggerInteraction.Ignore
        );

        bool pathBlocked = Physics.SphereCast(
            _clearanceOrigin.position,
            _collisionRadius,
            _aimDirection.forward,
            out _,
            distance,
            _collisionLayer,
            QueryTriggerInteraction.Ignore
        );

        bool blocked = originBlocked || pathBlocked;

        if (!_weaponBlocked)
        {
            _clearTimer = 0f;

            if (blocked)
            {
                _blockTimer += Time.deltaTime;

                if (_blockTimer >= _blockDelay)
                    SetWeaponBlocked(true);
            }
            else
            {
                _blockTimer = 0f;
            }
        }
        else
        {
            _blockTimer = 0f;

            if (!blocked)
            {
                _clearTimer += Time.deltaTime;

                if (_clearTimer >= _clearDelay)
                    SetWeaponBlocked(false);
            }
            else
            {
                _clearTimer = 0f;
            }
        }
    }

    private void SetWeaponBlocked(bool blocked)
    {
        if (_weaponBlocked == blocked)
            return;

        _weaponBlocked = blocked;
        _blockTimer = 0f;
        _clearTimer = 0f;

        float targetValue = blocked ? 0f : 1f;

        if (_gunBlendCoroutine != null)
            StopCoroutine(_gunBlendCoroutine);

        _gunBlendCoroutine = StartCoroutine(BlendHasGun(targetValue));
    }

    private IEnumerator BlendHasGun(float targetValue)
    {
        float startValue = _animator.GetFloat("HasGun");
        float elapsedTime = 0f;

        while (elapsedTime < _gunBlendDuration)
        {
            elapsedTime += Time.deltaTime;

            float blend = Mathf.Clamp01(elapsedTime / _gunBlendDuration);
            _animator.SetFloat("HasGun", Mathf.Lerp(startValue, targetValue, blend));

            yield return null;
        }

        _animator.SetFloat("HasGun", targetValue);
        _gunBlendCoroutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (_clearanceOrigin == null || _aimDirection == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            _clearanceOrigin.position + _aimDirection.forward * _blockDistance,
            _collisionRadius
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            _clearanceOrigin.position + _aimDirection.forward * _clearDistance,
            _collisionRadius
        );

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_clearanceOrigin.position, _collisionRadius);
    }
}
