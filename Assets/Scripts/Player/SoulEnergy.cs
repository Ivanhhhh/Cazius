using UnityEngine;

public class SoulEnergy : MonoBehaviour, IEInteractable
{
    [Header("Soul Energy")]
    [SerializeField] private float _maxSoulEnergy;

    [Header("UI")]
    [SerializeField] private string _interactText = "F to Grab Soul Energy";

    [Header("VFX")]
    [SerializeField] private GameObject _grabSoulEnergyVFX;
    [SerializeField] private float _vfxDestroyDelay = 1f;

    public void Interact(Transform interactorTransform)
    {
        if (KeyInventorySystem.Instance.CurrentSoulEnergy >= _maxSoulEnergy) return;

        KeyInventorySystem.Instance.AddSoulEnergy();

        if (SoulUIManager.Instance != null)
        {
            SoulUIManager.Instance.UpdateUI(KeyInventorySystem.Instance.CurrentSoulEnergy);
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

}




