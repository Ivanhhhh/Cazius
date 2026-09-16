using UnityEngine;
using UnityEngine.InputSystem;

public class OpenTheDoor : MonoBehaviour
{
    public Transform player;
    public bool isOpen;
    public bool playerInside;

    private Quaternion initialAngle;
    private Quaternion targetAngle;

    void Start()
    {
        initialAngle = transform.rotation;
        targetAngle = initialAngle;

        GameInputManager.Instance.Controls.Player.Interact.started += OpenDoor;
    }

    void Update()
    {
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetAngle,
            Time.deltaTime * 5f
        );
    }

    void OnDestroy()
    {
        if (GameInputManager.Instance != null)
        {
            GameInputManager.Instance.Controls.Player.Interact.started -= OpenDoor;
        }
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
}