using System.Collections;
using UnityEngine;

public class EnemyEntranceSequence : MonoBehaviour
{
    [SerializeField] private Animator fakeEnemyAnimator;
    [SerializeField] private string animationTriggerName = "Entrance";

    [SerializeField] private GameObject[] fakeEnemyObjectToDisable;
    [SerializeField] private GameObject realEnemyToEnable;

    [SerializeField] private float delayBeforeAnimation = 0f;
    [SerializeField] private float animationDuration = 2f;
    [SerializeField] private float delayBeforeRealEnemy = 0f;

    [SerializeField] private bool disableRealEnemyOnStart = true;
    [SerializeField] private bool disableFakeEnemyAtEnd = true;

    private Coroutine entranceRoutine;

    private void Awake()
    {
        if (fakeEnemyAnimator == null)
            fakeEnemyAnimator = GetComponentInChildren<Animator>();

        if (disableRealEnemyOnStart && realEnemyToEnable != null)
            realEnemyToEnable.SetActive(false);
    }

    private void OnEnable()
    {
        if (entranceRoutine != null)
            StopCoroutine(entranceRoutine);

        entranceRoutine = StartCoroutine(PlayEntranceRoutine());
    }

    private IEnumerator PlayEntranceRoutine()
    {
        if (delayBeforeAnimation > 0f)
            yield return new WaitForSeconds(delayBeforeAnimation);

        if (fakeEnemyAnimator != null && !string.IsNullOrEmpty(animationTriggerName))
        {
            fakeEnemyAnimator.ResetTrigger(animationTriggerName);
            fakeEnemyAnimator.SetTrigger(animationTriggerName);
        }

        yield return new WaitForSeconds(animationDuration);

        if (delayBeforeRealEnemy > 0f)
            yield return new WaitForSeconds(delayBeforeRealEnemy);

        if (realEnemyToEnable != null)
        {
            realEnemyToEnable.transform.position = transform.position;
            realEnemyToEnable.transform.rotation = transform.rotation;
            realEnemyToEnable.SetActive(true);

        }

        if (disableFakeEnemyAtEnd && fakeEnemyObjectToDisable != null)
        {
            foreach (var o in fakeEnemyObjectToDisable)
            {
                o.SetActive(false);
            }
        }
    }
}