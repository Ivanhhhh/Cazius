using System.Collections;
using UnityEngine;

public class TriggerAnimatorOnDisable : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;
    [SerializeField] private string triggerName = "BreakFloor";

    [SerializeField] private float delayAfterTrigger = 1.5f;
    [SerializeField] private GameObject objectToEnableAfterDelay;

    [SerializeField] private bool findAnimatorIfMissing = true;
    [SerializeField] private string animatorObjectName = "FloorAnimator";

    [Header("Sound")]
    [SerializeField] private SFXManager.SFXCategoryType sfxType;
    [SerializeField] private SFXManager.SFXCategoryType sfxTypeSMOKE;

    [SerializeField] private GameObject smokeParticlee;


    [Header("Camera Shake")]
    [SerializeField] private float shakeStrength = 0.08f;

    [Header("Particle System")]
    [SerializeField] private ParticleSystem particle_system;

    private int _triggerHash;
    private bool _applicationQuitting;
    private bool _alreadyTriggered;

    private void Awake()
    {
        _triggerHash = Animator.StringToHash(triggerName);
    }

    private void Start()
    {
        TryFindAnimator();
    }

    private void OnDisable()
    {
        if (_applicationQuitting) return;
        if (_alreadyTriggered) return;

        _alreadyTriggered = true;

        TryFindAnimator();

        if (targetAnimator == null)
        {
            Debug.LogWarning($"{name}: No Animator found.");
            return;
        }

        targetAnimator.SetTrigger(_triggerHash);

        GameObject animatorObjectToDisable = targetAnimator.gameObject;

        DelayedActionRunner.Run(DelayedSwap(animatorObjectToDisable));
    }

    private IEnumerator DelayedSwap(GameObject objectToDisable)
    {
        SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxType, transform.position);
        SFXManager.Instance.PlaySFXAtPositionAndPauseMusic(sfxTypeSMOKE, transform.position);

        particle_system.Play();
        smokeParticlee.SetActive(true);

        CameraShake.Instance.StartShake(shakeStrength);

        yield return new WaitForSeconds(delayAfterTrigger);

        CameraShake.Instance.StopShake();

        if (objectToDisable != null)
            objectToDisable.SetActive(false);

        if (objectToEnableAfterDelay != null)
            objectToEnableAfterDelay.SetActive(true);
    }

    private void TryFindAnimator()
    {
        if (targetAnimator != null) return;
        if (!findAnimatorIfMissing) return;

        GameObject foundObject = GameObject.Find(animatorObjectName);

        if (foundObject != null)
            targetAnimator = foundObject.GetComponent<Animator>();
    }

    private void OnApplicationQuit()
    {
        _applicationQuitting = true;
    }

    private class DelayedActionRunner : MonoBehaviour
    {
        private static DelayedActionRunner _instance;

        public static void Run(IEnumerator routine)
        {
            if (_instance == null)
            {
                GameObject runnerObject = new GameObject("Delayed Action Runner");
                DontDestroyOnLoad(runnerObject);
                _instance = runnerObject.AddComponent<DelayedActionRunner>();
            }

            _instance.StartCoroutine(routine);
        }
    }
}