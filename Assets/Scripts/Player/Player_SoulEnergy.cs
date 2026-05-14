using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_SoulEnergy : MonoBehaviour, IInteractable
{
    [Header("Soul Energy")]
    [SerializeField] private float _currentSoulEnergy;
    [SerializeField] private float _maxSoulEnergy;

    [Header("UI")]
    [SerializeField] private GameObject[] _soulEnergyPanel;
    [SerializeField] private GameObject _soulEnergyInteractionIcon;

    public void Interact(InventorySystem inventory)
    {
        if (inventory.CurrentSoulEnergy >= _maxSoulEnergy)
        {
            Debug.Log("Enough Soul Energy");
            return;
        }

        inventory.AddSoulEnergy();

        UpdatePanelUI(inventory);

        _soulEnergyInteractionIcon.SetActive(false);

        Destroy(gameObject);
    }
    private void UpdatePanelUI(InventorySystem inventory)
    {
        foreach (GameObject panel in _soulEnergyPanel)
        {
            panel.SetActive(false);
        }

        int _currentSoul = inventory.CurrentSoulEnergy;

        if (_currentSoul > 0)
        {
            _soulEnergyPanel[_currentSoul - 1].SetActive(true);
        }
    }
    void OnEnable()
    {
        _soulEnergyInteractionIcon.SetActive(true);
    }
    void OnDisable()
    {
        _soulEnergyInteractionIcon.SetActive(false);
    }


}
