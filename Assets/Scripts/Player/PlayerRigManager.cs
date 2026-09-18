using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PlayerRigManager : MonoBehaviour
{

    [SerializeField] Rig _rig;
    [SerializeField] Animator _animator;

    [Header("MultiAimConstraints")]

    [SerializeField] MultiAimConstraint _hipsAim;
    [SerializeField] MultiAimConstraint _hipsAim2;
    [SerializeField] MultiAimConstraint _headAim;

    [Header("AimingSettings")]

    [SerializeField] float _hipsAimingWeight = 1f;
    [SerializeField] float _hips2AimingWeight = 1f;
    [SerializeField] float _headAimingWeight = 1f;
    [SerializeField] float _HeadAimingSourceObjWeight = 0.42f;

    [Header("WalkingSettings")]

    [SerializeField] float _hipsWalkingWeight = 0.742f;
    [SerializeField] float _hips2WalkingWeight = 0.742f;
    [SerializeField] float _headWalkingWeight = 1f;
    [SerializeField] float _HeadWalkingSourceObjWeight = 0.58f;

    [Header("Headbutt")]

    [SerializeField] float _headbuttDuration = 0.66f;
    private Coroutine _headbuttCoroutine;

    public void SetAimingConstraints()
    {
        _hipsAim.weight = _hipsAimingWeight;
        _hipsAim2.weight = _hips2AimingWeight;
        _headAim.weight = _headAimingWeight;
        //_headAim.data.sourceObjects.SetWeight(0, _HeadAimingSourceObjWeight);

        var headSource = _headAim.data.sourceObjects;

        headSource.SetWeight(0, _HeadAimingSourceObjWeight);

        _headAim.data.sourceObjects = headSource;
    }

    public void SetwalkingConstraints()
    {
        _hipsAim.weight = _hipsWalkingWeight;
        _hipsAim2.weight = _hips2WalkingWeight;
        _headAim.weight = _headWalkingWeight;
        //_headAim.data.sourceObjects.SetWeight(0, _HeadWalkingSourceObjWeight);

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

    public void TryHeadbutt()
    {
        if (_headbuttCoroutine != null)
            { return; }

        _headbuttCoroutine = StartCoroutine(HeadbuttCoroutine());
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
}

