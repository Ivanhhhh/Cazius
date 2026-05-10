using UnityEngine;

public class EnemyActivator : MonoBehaviour, IInteractable // Lo que queres activar cuando agaarras la llave
{
    [SerializeField] private GameObject[] _enemiesToActivate;

    public void Interact(InventorySystem inventory)
    {
        foreach (GameObject enemy in _enemiesToActivate)
        {
            enemy.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
