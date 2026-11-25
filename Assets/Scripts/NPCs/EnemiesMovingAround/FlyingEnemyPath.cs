
using System.Collections;
using UnityEngine;

public class FlyingEnemyPath : MonoBehaviour
{
    public enum RotationMode
    {
        Smooth,
        Instant,
        Keep
    }

    [Header("Enemy")]
    [SerializeField] private Transform _enemyModel;

    [Header("Waypoints")]
    [SerializeField] private Transform[] _waypoints;

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private RotationMode _rotationMode = RotationMode.Smooth;

    private Coroutine _flightRoutine;

    private void Awake()
    {
        if (_enemyModel != null)
            _enemyModel.gameObject.SetActive(false);
    }

    public void Play()
    {
        if (_enemyModel == null || _waypoints == null || _waypoints.Length < 2)
            return;

        foreach (Transform point in _waypoints)
        {
            if (point == null)
                return;
        }

        if (_flightRoutine != null)
            StopCoroutine(_flightRoutine);

        _enemyModel.SetPositionAndRotation(
            _waypoints[0].position,
            _waypoints[0].rotation
        );

        _enemyModel.gameObject.SetActive(true);

        _flightRoutine = StartCoroutine(FlyRoutine());
    }

    private IEnumerator FlyRoutine()
    {
        for (int i = 1; i < _waypoints.Length; i++)
        {
            Vector3 startPosition = _enemyModel.position;
            Vector3 targetPosition = _waypoints[i].position;

            Quaternion startRotation = _enemyModel.rotation;
            Quaternion targetRotation = _waypoints[i].rotation;

            float distance = Vector3.Distance(startPosition, targetPosition);
            float duration = distance / Mathf.Max(_moveSpeed, 0.01f);

            if (_rotationMode == RotationMode.Instant)
                _enemyModel.rotation = targetRotation;

            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                float t = Mathf.Clamp01(elapsed / duration);

                _enemyModel.position = Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );

                if (_rotationMode == RotationMode.Smooth)
                {
                    _enemyModel.rotation = Quaternion.Slerp(
                        startRotation,
                        targetRotation,
                        t
                    );
                }

                yield return null;
            }

            _enemyModel.position = targetPosition;

            if (_rotationMode == RotationMode.Smooth)
                _enemyModel.rotation = targetRotation;
        }

        _enemyModel.gameObject.SetActive(false);
        _flightRoutine = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (_waypoints == null)
            return;

        Gizmos.color = Color.cyan;

        for (int i = 0; i < _waypoints.Length; i++)
        {
            Transform point = _waypoints[i];

            if (point == null)
                continue;

            Gizmos.DrawSphere(point.position, 0.15f);

            Gizmos.DrawRay(
                point.position,
                point.forward * 0.7f
            );

            if (i < _waypoints.Length - 1 && _waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(
                    point.position,
                    _waypoints[i + 1].position
                );
            }
        }
    }
}
