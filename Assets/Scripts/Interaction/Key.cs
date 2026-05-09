using UnityEngine;

public class Key : MonoBehaviour, IInteractable // Q hay q activar
{
    [SerializeField] private GameObject[] _enemiesToActivate;

    public void Interact(InventorySystem inventory)
    {
        inventory.AddKey();

        foreach (GameObject enemy in _enemiesToActivate)
        {
            enemy.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
