using System.Collections;
using UnityEngine;

public class SoundSteps : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private float _stepInterval = 0.05f;

    private float _timer = 0.1f;

    void Update()
    {
        Debug.Log(_player._currentSpeed);
        if (_player._currentSpeed > 0.01)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.StepsSFX,transform.position);
                _timer = _stepInterval;
            }
        }
        else
        {
            _timer = 0.1f;
        }
    }
}