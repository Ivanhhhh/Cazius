using System.Collections;
using UnityEngine;

public class SoundSteps : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private float _stepInterval = 0.05f;

    [SerializeField] private float Volume;

    private float _timer = 0.1f;

    void Update()
    {
        if (_player._currentSpeed > 0.01)
        {
            _timer -= Time.deltaTime;

            if (_timer <= 0f)
            {
                //SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.StepsSFX,transform.position);
                SFXManager.Instance.PlaySFX(SFXManager.SFXCategoryType.StepsSFX,Volume);
                _timer = _stepInterval;
            }
        }
        else
        {
            _timer = 0.1f;
        }
    }
}