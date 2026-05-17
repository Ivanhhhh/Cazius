using UnityEngine;

public class Player_SoulEnergy : MonoBehaviour, IInteractable
{
    [Header("Soul Energy")]
    [SerializeField] private float _maxSoulEnergy;

    [Header("UI")]
    [SerializeField] private GameObject _soulEnergyInteractionIcon;

    public void Interact(InventorySystem inventory)
    {
        if (inventory.CurrentSoulEnergy >= _maxSoulEnergy) return;

        inventory.AddSoulEnergy();

        if (SoulUIManager.Instance != null)
        {
            SoulUIManager.Instance.UpdateUI(inventory.CurrentSoulEnergy);
        }

        Destroy(gameObject);
    }

    void OnEnable() { _soulEnergyInteractionIcon.SetActive(true); }

}




