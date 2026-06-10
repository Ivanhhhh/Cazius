using UnityEngine;

public class TriggerAnimatorOnDisable : MonoBehaviour
{
    [SerializeField] private Animator targetAnimator;

    [SerializeField] private string triggerName = "BreakFloor";

    [SerializeField] private bool findAnimatorIfMissing = true;
    [SerializeField] private string animatorObjectName = "FloorAnimator";

    private void Start()
    {
        targetAnimator = GameObject.Find(animatorObjectName).GetComponent<Animator>();
    }

    private void OnDisable()
    {
        if (targetAnimator == null && findAnimatorIfMissing)
        {
            GameObject foundObject = GameObject.Find(animatorObjectName);

            if (foundObject != null)
                targetAnimator = foundObject.GetComponent<Animator>();
        }

        if (targetAnimator == null)
        {
            Debug.Log("NoAnimatorFound");
            return;
        }

        targetAnimator.SetTrigger(triggerName);
    }
}