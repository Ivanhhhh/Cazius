using UnityEngine;

public class NPCInteractable : MonoBehaviour, IEInteractable
{
    public enum GiveType { Health, Ammo }

    [SerializeField] private string _interactText = "E to talk";
    [SerializeField] private string _wavingTrigger = "Waving";
    [SerializeField] private GiveType _giveType;
    [SerializeField] private ItemData _health;
    [SerializeField] private ItemData _ammo;

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

        switch (_giveType)
        {
            case GiveType.Health:
                Inventory.Instance.AddItem(_health);
                break;
            case GiveType.Ammo:
                Inventory.Instance.AddItem(_ammo);
                break;
        }

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
