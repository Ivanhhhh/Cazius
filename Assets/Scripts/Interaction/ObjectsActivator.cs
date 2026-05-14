using UnityEngine;

public class ObjectsActivator : MonoBehaviour, IInteractable // Lo que queres activar cuando agaarras la llave
{
    [SerializeField] private GameObject[] _ObjectsToActivate;

    public void Interact(InventorySystem inventory)
    {
        foreach (GameObject objects in _ObjectsToActivate)
        {
            objects.SetActive(true);
        }

        //gameObject.SetActive(false);
    }
}
