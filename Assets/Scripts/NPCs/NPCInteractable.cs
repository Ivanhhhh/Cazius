using UnityEngine;

public class NPCInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private string _wavingTrigger = "Waving";

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Interact(Transform interactorTransform)
    {
        Debug.Log("Interaction!");

        _animator.SetTrigger(_wavingTrigger);

        // Give Bullets

        // Hide Interact UI

        // VFX

    }

}
