using UnityEngine;
public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField] private PlayerMovement _player;
    [SerializeField] private float _stepInterval = 0.05f;

    private float _timer;

    void Update()
    {
        if (_player._currentSpeed > 0.01f)
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
            _timer = 0f;
        }
    }
}