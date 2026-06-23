using UnityEngine;

public class TeleportOnTrigger : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string _targetTag = "Pickable Objects";

    [Header("Destination")]
    [SerializeField] private Transform spawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_targetTag))
        {
            other.transform.position = spawnPoint.position;
            other.transform.rotation = spawnPoint.rotation;

        }
    }
}
