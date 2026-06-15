using System.Collections;
using UnityEngine;

public class WhenOpenDoor : MonoBehaviour
{
    [Header("Door Animation")]
    [SerializeField] private Animator _doorAnimator;
    [SerializeField] private GameObject _currentCamera;
    [SerializeField] private GameObject _secondCamera;

    [Header("Camera Position")]
    [SerializeField] private Transform _cameraPoint;

    void Awake()
    {
        _doorAnimator.enabled = false;
    }

    public IEnumerator WhenKeyOpenDoor(string animationName)
    {
        _currentCamera.SetActive(false);

        _secondCamera.transform.position = _cameraPoint.position;
        _secondCamera.transform.rotation = _cameraPoint.rotation;
        _secondCamera.SetActive(true);

        _doorAnimator.enabled = true;
        _doorAnimator.Play(animationName, 0, 0f);

        yield return new WaitForSeconds(3f);

        _doorAnimator.enabled = false;
        _secondCamera.SetActive(false);
        _currentCamera.SetActive(true);
    }

}


