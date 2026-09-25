using UnityEngine;
using UnityEngine.InputSystem;

public class OpenTheDoor : MonoBehaviour, IEInteractable
{
    public Transform player;
    public bool isOpen;
    public bool playerInside;

    private Quaternion initialAngle;
    private Quaternion targetAngle;

    [Header("Interact")]
    [SerializeField] private string _interactText = "F to Open Door";
    [SerializeField] private Transform _interactionUIPoint;

    void Start()
    {
        initialAngle = transform.rotation;
        targetAngle = initialAngle;

    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetAngle,
            Time.deltaTime * 5f
        );
    }

    private void OpenDoor(InputAction.CallbackContext context)
    {
        if (!playerInside)
            return;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        if (player == null)
            return;

        if (!isOpen)
        {
            Vector3 direction = player.position - transform.position;

            if (Vector3.Dot(transform.right, direction) > 0)
            {
                targetAngle = initialAngle * Quaternion.Euler(0, -90, 0);
            }
            else
            {
                targetAngle = initialAngle * Quaternion.Euler(0, 90, 0);
            }

            isOpen = true;
        }
        else
        {
            targetAngle = initialAngle;
            isOpen = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            player = null;
        }
    }

    public void Interact(Transform interactorTransform)
    {
        OpenDoor(default);
    }

    public Transform GetInteractionUIPoint()
    {
        return _interactionUIPoint != null
            ? _interactionUIPoint
            : transform;
    }
    public string GetInteractText()
    {
        return _interactText;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public bool IsLocked()
    {
        return false;
    }
}