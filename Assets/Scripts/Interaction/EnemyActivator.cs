using UnityEngine;

public class EnemyActivator : MonoBehaviour, IInteractable // Lo que queres activar cuando agaarras la llave
{
    [SerializeField] private GameObject[] _enemiesToActivate;

    public void Interact(InventorySystem inventory)
    {
        Debug.Log("AGARRE LA  LLAVE Y SPAAWNE EL ENEMIGO");
        foreach (GameObject enemy in _enemiesToActivate)
        {
            enemy.SetActive(true);
        }

        gameObject.SetActive(false);
    }
}
