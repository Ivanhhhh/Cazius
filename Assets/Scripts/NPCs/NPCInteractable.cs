using UnityEngine;

public class NPCInteractable : MonoBehaviour, IEInteractable
{
    [SerializeField] private string _interactText = "E to talk";
    [SerializeField] private string _wavingTrigger = "Waving";

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(Transform interactorTransform)
    {
        RotateTowardsPlayer(interactorTransform);
        _animator.SetTrigger(_wavingTrigger);
        SFXManager.Instance.PlaySFXAtPosition(SFXManager.SFXCategoryType.MaleHeySFX, transform.position);

        // Give Bullets

    }
    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

    private void RotateTowardsPlayer(Transform interactorTransform)
    {
        Vector3 dir = interactorTransform.position - transform.position;
        dir.y = 0;
        transform.rotation = Quaternion.LookRotation(dir);
    }


}
