using UnityEngine;

public class Key : MonoBehaviour, IInteractable // Lo tiene la "Key" con layer "Interactive", _keyPanel tiene q estar apagado
{
    [SerializeField] private GameObject[] _enemiesToActivate;

    [Header("UI")]
    [SerializeField] private GameObject _keyPanel;
    [SerializeField] private GameObject _interactionIcon;

    public void Interact(InventorySystem inventory)
    {
        inventory.AddKey();

        _keyPanel.SetActive(true);
        _interactionIcon.SetActive(false);

        foreach (GameObject enemy in _enemiesToActivate)
        {
            enemy.SetActive(true);
        }

        gameObject.SetActive(false);
    }

    public void ShowIcon()
    {
        _interactionIcon.SetActive(true);
    }

    public void HideIcon()
    {
        _interactionIcon.SetActive(false);
    }
}
