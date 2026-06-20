using System.Collections;
using UnityEngine;

public class WhenOpenDoor : MonoBehaviour
{
    [Header("Door Animation")]
    [SerializeField] private Animator _doorAnimator;
    [SerializeField] private Transform _currentCamera;

    [Header("Camera Position")]
    [SerializeField] private Transform _cameraPoint;

    [Header("Camera Settings")]
    [SerializeField] private float _cameraMoveDuration = 1f;

    void Awake()
    {
        _doorAnimator.enabled = false;
    }

    public IEnumerator WhenKeyOpenDoor(string animationName)
    {
        Vector3 originalPosition = _currentCamera.position;
        Quaternion originalRotation = _currentCamera.rotation;

        yield return StartCoroutine(LerpCamera(
            originalPosition,
            _cameraPoint.position,
            originalRotation,
            _cameraPoint.rotation
        ));

        _doorAnimator.enabled = true;
        _doorAnimator.Play(animationName, 0, 0f);

        yield return new WaitForSeconds(3f);

        _doorAnimator.enabled = false;

        yield return StartCoroutine(LerpCamera(
            _cameraPoint.position,
            originalPosition,
            _cameraPoint.rotation,
            originalRotation
        ));
    }

    private IEnumerator LerpCamera(
        Vector3 startPos,
        Vector3 endPos,
        Quaternion startRot,
        Quaternion endRot)
    {
        float elapsed = 0f;

        while (elapsed < _cameraMoveDuration)
        {
            float t = elapsed / _cameraMoveDuration;

            _currentCamera.position = Vector3.Lerp(startPos, endPos, t);
            _currentCamera.rotation = Quaternion.Slerp(startRot, endRot, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        _currentCamera.position = endPos;
        _currentCamera.rotation = endRot;
    }
}


