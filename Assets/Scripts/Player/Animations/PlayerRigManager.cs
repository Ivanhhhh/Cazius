using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PlayerRigManager : MonoBehaviour
{

    [SerializeField] Rig _rig;
    [SerializeField] Animator _animator;
    [SerializeField] PlayerModelMovement _playerModelMovement;

    [Header("MultiAimConstraints")]

    [SerializeField] MultiAimConstraint _hipsAim;
    [SerializeField] MultiAimConstraint _hipsAim2;
    [SerializeField] MultiAimConstraint _headAim;

    [Header("AimingSettings")]

    [SerializeField] float _hipsAimingWeight = 1f;
    [SerializeField] float _hipsAimingMaxLimit = 31f;
    [SerializeField] float _hips2AimingWeight = 1f;
    [SerializeField] float _headAimingWeight = 1f;
    [SerializeField] float _HeadAimingSourceObjWeight = 0.42f;

    [Header("WalkingSettings")]

    [SerializeField] float _hipsWalkingWeight = 0.742f;
    [SerializeField] float _hipsWalkingMaxLimit = 10f;
    [SerializeField] float _hips2WalkingWeight = 0.742f;
    [SerializeField] float _headWalkingWeight = 1f;
    [SerializeField] float _HeadWalkingSourceObjWeight = 0.58f;

    [Header("Headbutt")]

    [SerializeField] float _headbuttDuration = 0.66f;
    private Coroutine _headbuttCoroutine;

    public void SetAimingConstraints()
    {
        _playerModelMovement.isAiming = true;

        _hipsAim.weight = _hipsAimingWeight;
        _hipsAim2.weight = _hips2AimingWeight;
        _headAim.weight = _headAimingWeight;

        Vector2 hipsSource = _hipsAim.data.limits;

        hipsSource = new Vector2(hipsSource.x, _hipsAimingMaxLimit);

        _hipsAim.data.limits = hipsSource;

        var headSource = _headAim.data.sourceObjects;

        headSource.SetWeight(0, _HeadAimingSourceObjWeight);

        _headAim.data.sourceObjects = headSource;
    }

    public void SetwalkingConstraints()
    {
        _playerModelMovement.isAiming = false;

        _hipsAim.weight = _hipsWalkingWeight;
        _hipsAim2.weight = _hips2WalkingWeight;
        _headAim.weight = _headWalkingWeight;

        Vector2 hipsSource = _hipsAim.data.limits;

        hipsSource = new Vector2(hipsSource.x, _hipsWalkingMaxLimit);

        _hipsAim.data.limits = hipsSource;

        var headSource = _headAim.data.sourceObjects;

        headSource.SetWeight(0, _HeadWalkingSourceObjWeight);

        _headAim.data.sourceObjects = headSource;
    }

    public void DisableRig()
    {
        _rig.weight = 0f;
    }

    public void EnableRig()
    {
        _rig.weight = 1f;
    }

    public void TryHeadbutt(bool lookTowards, Transform posToLookTowards)
    {
        if (_headbuttCoroutine != null)
            { return; }

        if (lookTowards)
        {
            _headbuttCoroutine = StartCoroutine(HeadbuttCoroutineWithLookTowards(posToLookTowards));
        }
        else
        {
            _headbuttCoroutine = StartCoroutine(HeadbuttCoroutine());
        }
    }

    public void ForceLookTowards(bool force, Transform towards)
    {
        _playerModelMovement.ForceToLookForward(force, towards);
    }

    private IEnumerator HeadbuttCoroutine()
    {
        DisableRig();

        _animator.SetTrigger("Headbutt");

        yield return new WaitForSeconds(_headbuttDuration / 2f);

        GameManager.Instance.cameraShake.DamageShake();

        yield return new WaitForSeconds(_headbuttDuration / 2f);

        EnableRig();

        _headbuttCoroutine = null;

    }

    private IEnumerator HeadbuttCoroutineWithLookTowards(Transform posToLookTowards)
    {
        DisableRig();

        _playerModelMovement.ForceToLookForward(true, posToLookTowards);


        _animator.SetTrigger("Headbutt");

        yield return new WaitForSeconds(_headbuttDuration / 2f);

        GameManager.Instance.cameraShake.DamageShake();

        yield return new WaitForSeconds(_headbuttDuration / 2f);


        _playerModelMovement.ForceToLookForward(false, null);

        EnableRig();

        _headbuttCoroutine = null;

    }
}

