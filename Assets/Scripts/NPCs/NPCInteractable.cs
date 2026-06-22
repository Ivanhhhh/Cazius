using TMPro;
using UnityEngine;

public class NPCInteractable : MonoBehaviour, IEInteractable
{
    public enum GiveType { Health, Ammo, Scrap }

    [SerializeField] private string _interactText = "F to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private ItemData _itemToGive;
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

        Player_AimAndShoot player = interactorTransform.GetComponentInParent<Player_AimAndShoot>();

        if (player == null) return;

        Inventory.Instance.AddItem(_itemToGive);

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
