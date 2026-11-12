using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class SlidingShopDoor : MonoBehaviour
{
    public enum SlideAxis { X, Y, Z }

    [Header("Reference")]
    [Tooltip("If empty moves this GameObject")]
    [SerializeField] private Transform _doorTransform;

    [Header("Slide Settings")]
    [SerializeField] private SlideAxis _slideAxis = SlideAxis.X;
    [SerializeField] private float _slideDistance = 2f;
    [SerializeField] private float _slideSpeed = 4f;
    [SerializeField] private float _closeDelay = 3f;

    [Header("Trigger Settings")]
    [SerializeField] private string _playerTag = "Player";

    [Header("Audio (Optional)")]
    [SerializeField] private bool _playSFX = false;
    [SerializeField] private SFXManager.SFXCategoryType _openSFX;
    [SerializeField] private SFXManager.SFXCategoryType _closeSFX;

    private Vector3 _closedPosition;
    private Vector3 _openPosition;
    private Coroutine _slideCoroutine;
    private Coroutine _closeDelayCoroutine;

    private Transform TargetTransform => _doorTransform != null ? _doorTransform : transform;

    private void Reset()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }

    private void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;

        _closedPosition = TargetTransform.localPosition;
        _openPosition = _closedPosition + GetAxisOffset();
    }

    private Vector3 GetAxisOffset()
    {
        return _slideAxis switch
        {
            SlideAxis.X => new Vector3(_slideDistance, 0f, 0f),
            SlideAxis.Y => new Vector3(0f, _slideDistance, 0f),
            SlideAxis.Z => new Vector3(0f, 0f, _slideDistance),
            _ => Vector3.zero
        };
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;

        if (_closeDelayCoroutine != null)
        {
            StopCoroutine(_closeDelayCoroutine);
            _closeDelayCoroutine = null;
        }

        if (_playSFX && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFXAtPosition(_openSFX, TargetTransform.position);

        StartSlide(_openPosition);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(_playerTag)) return;

        if (_closeDelayCoroutine != null)
            StopCoroutine(_closeDelayCoroutine);

        _closeDelayCoroutine = StartCoroutine(CloseAfterDelayRoutine());
    }

    private IEnumerator CloseAfterDelayRoutine()
    {
        yield return new WaitForSeconds(_closeDelay);
        _closeDelayCoroutine = null;

        if (_playSFX && SFXManager.Instance != null)
            SFXManager.Instance.PlaySFXAtPosition(_closeSFX, TargetTransform.position);

        StartSlide(_closedPosition);
    }

    private void StartSlide(Vector3 targetPos)
    {
        if (_slideCoroutine != null)
            StopCoroutine(_slideCoroutine);

        _slideCoroutine = StartCoroutine(SlideRoutine(targetPos));
    }

    private IEnumerator SlideRoutine(Vector3 targetPos)
    {
        while (Vector3.Distance(TargetTransform.localPosition, targetPos) > 0.001f)
        {
            TargetTransform.localPosition = Vector3.MoveTowards(
                TargetTransform.localPosition,
                targetPos,
                _slideSpeed * Time.deltaTime
            );
            yield return null;
        }

        TargetTransform.localPosition = targetPos;
        _slideCoroutine = null;
    }

}
