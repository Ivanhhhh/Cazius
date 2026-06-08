using UnityEngine;

public class PlaySFXOnDisable : MonoBehaviour
{
    [SerializeField] private SFXManager.SFXCategoryType _sfxCategory = SFXManager.SFXCategoryType.RockCrumble;

    [SerializeField] private Transform _playAtPosition;

    [SerializeField] private bool _playOnlyOnce = true;
    [SerializeField] private bool _ignoreOnApplicationQuit = true;

    private bool _hasPlayed;
    private static bool _isQuitting;

    private void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    private void OnDisable()
    {
        if (_ignoreOnApplicationQuit && _isQuitting)
            return;

        if (_playOnlyOnce && _hasPlayed)
            return;

        if (SFXManager.Instance == null)
        {
            Debug.LogWarning("SFXManager Not Found");
            return;
        }

        Vector3 position = _playAtPosition != null
            ? _playAtPosition.position
            : transform.position;

        SFXManager.Instance.PlaySFXAtPosition(_sfxCategory, position);

        _hasPlayed = true;
    }
}