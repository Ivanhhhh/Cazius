using UnityEngine;

public class PickSoulEnergy : MonoBehaviour, IEInteractable
{
    [Header("Soul Energy")]
    [SerializeField] private float _maxSoulEnergy;

    [Header("UI")]
    [SerializeField] private string _interactText = "F to Grab Soul Energy";

    [Header("VFX")]
    [SerializeField] private GameObject _grabSoulEnergyVFX;
    [SerializeField] private float _vfxDestroyDelay = 1f;

    [SerializeField] private Transform _interactionUIPoint;

    [SerializeField]SoulRandomAmount _sra;
    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        _sra.RandomSoul();
        print ("Door");

        if (_grabSoulEnergyVFX != null)
        {
            GameObject vfx = Instantiate(_grabSoulEnergyVFX, transform.position, transform.rotation);

            Destroy(vfx, _vfxDestroyDelay);
        }

        Destroy(gameObject);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

    public bool IsLocked() { return false; }

}




