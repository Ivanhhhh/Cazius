using UnityEngine;

public class PickSoulEnergy : MonoBehaviour, IEInteractable
{

    [Header("UI")]
    [SerializeField] private string _interactText = "F to Grab Soul Energy";

    [Header("VFX")]
    [SerializeField] private GameObject _grabSoulEnergyVFX;
    [SerializeField] private float _vfxDestroyDelay = 1f;

    [SerializeField] private Transform _interactionUIPoint;
     
     [SerializeField] SoulRandomAmount _randomAmountSoulEnergy;

      public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }

    public void Interact(Transform interactorTransform)
    {
        print ("interact");
         _randomAmountSoulEnergy.RandomSoul();

        if (SoulUIManager.Instance != null)
        {
            SoulUIManager.Instance.UpdateUI(SoulEnergyManager.Instance.CurrentSoulEnergy);
        }


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


