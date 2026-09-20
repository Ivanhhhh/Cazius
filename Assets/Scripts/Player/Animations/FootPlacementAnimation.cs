using UnityEngine;

public class FootPlacementAnimation : MonoBehaviour
{
    [SerializeField] Animator _animator;

    [SerializeField] LayerMask _layerMask;

    [SerializeField] float _distanceToGround;

    private void OnAnimatorIK(int layerIndex)
    {
        if (_animator.GetBool("IsMoving"))
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("IKLeftFootWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat("IKLeftFootWeight"));
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("IKRIghtFootWeight"));
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat("IKRIghtFootWeight"));
        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);
        }



        //left foot

        RaycastHit hit;
        Ray ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hit, _distanceToGround + 1f, _layerMask))
        {
            Vector3 footPosition = hit.point;
            footPosition.y += _distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);

            _animator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hit.normal));
        }



        // right foot


        RaycastHit hitt;
        ray = new Ray(_animator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

        if (Physics.Raycast(ray, out hitt, _distanceToGround + 1f, _layerMask))
        {
            Vector3 footPos = hitt.point;
            footPos.y += _distanceToGround;
            _animator.SetIKPosition(AvatarIKGoal.RightFoot, footPos);

            _animator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hitt.normal));
        }

    }
}
