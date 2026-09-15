using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    public OpenTheDoor door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.playerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door.playerInside = false;
        }
    }
}
