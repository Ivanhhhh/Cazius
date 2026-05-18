using UnityEngine;

public class SoulEnergy : MonoBehaviour, IEInteractable
{
    [Header("Soul Energy")]
    [SerializeField] private float _maxSoulEnergy;

    [Header("UI")]
    [SerializeField] private string _interactText = "F to Grab Soul Energy";

    public void Interact(Transform interactorTransform)
    {
        if (KeyInventorySystem.Instance.CurrentSoulEnergy >= _maxSoulEnergy) return;

        KeyInventorySystem.Instance.AddSoulEnergy();

        if (SoulUIManager.Instance != null)
        {
            SoulUIManager.Instance.UpdateUI(KeyInventorySystem.Instance.CurrentSoulEnergy);
        }

        Destroy(gameObject);
    }

    public string GetInteractText() { return _interactText; }

    public Transform GetTransform() { return transform; }

}




