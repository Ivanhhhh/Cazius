using UnityEngine;

public class DoorStop : MonoBehaviour
{
    public bool Stop;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            Stop = true;
            print(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Door"))
        {
            Stop = false;
            print(other.gameObject);

        }
    }
}
