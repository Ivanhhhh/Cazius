using TMPro;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private float rotateVelocity;
    [SerializeField] private LayerMask interactiveLayerMask;

    [SerializeField] private GameObject currentObject;
    [SerializeField] private bool isInspecting;

    private GameObject inspectionInstance;

    [SerializeField] private Transform inspectionPoint;

    [SerializeField] private GameObject canvasUI;
    [SerializeField] private GameObject panelUI;
    [SerializeField] private GameObject KeyPanel;

    [SerializeField] private Transform interactiveIcon;
    [SerializeField] private Transform DoorIconLock;
    [SerializeField] private Transform DoorIconUnlock;
    [SerializeField] private Vector3 iconOffset;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera inspectionCamera;

    [SerializeField] private bool hasKey;
    [SerializeField] private GameObject keyOffset;
    [SerializeField] private GameObject doorToUnlock;
    [SerializeField] private GameObject doorToUnlock2;
    [SerializeField] private bool IsInsideTrigger = false;

    void Start()
    {
        inspectionCamera.enabled = false;
        canvasUI.SetActive(false);
        panelUI.SetActive(false);
        KeyPanel.SetActive(false);
        DoorIconLock.gameObject.SetActive(false);
        DoorIconUnlock.gameObject.SetActive(false);
    }

    void Update()
    {
        RaycastHit hit;
        Vector3 origin = transform.position;
        Vector3 direction = transform.forward;

        if (!isInspecting)
        {
            if (!hasKey)
            {
                interactiveIcon.gameObject.SetActive(true);
                Debug.DrawRay(origin, direction);

                if (Physics.Raycast(origin, direction, out hit, maxDistance, interactiveLayerMask))
                {
                    currentObject = hit.transform.gameObject;
                    interactiveIcon.gameObject.SetActive(true);
                    interactiveIcon.position = currentObject.transform.position + iconOffset;
                }
            }
            if (Input.GetKeyDown(KeyCode.F) && currentObject != null && !isInspecting)
            {
                hasKey = true;
                KeyPanel.SetActive(true);
                currentObject.gameObject.SetActive(false);
                //currentObject.transform.SetParent(keyOffset.transform);
                currentObject.transform.localPosition = Vector3.zero;
                interactiveIcon.gameObject.SetActive(false);

            }
            if (Input.GetKeyDown(KeyCode.F) && IsInsideTrigger && hasKey)
            {
                Debug.Log("Unlock");
                DoorIconUnlock.gameObject.SetActive(false);
                KeyPanel.SetActive(false); // OPCIONAL
                currentObject.gameObject.SetActive(false);
                doorToUnlock.transform.Rotate(0, -90, 0);
                Destroy(doorToUnlock);
                if (doorToUnlock != null)
                {
                    Destroy(doorToUnlock2);
                }
                //hasKey = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.I) && currentObject != null && !isInspecting)
        {
            isInspecting = true;
            currentObject.SetActive(true);

            mainCamera.enabled = false;
            inspectionCamera.enabled = true;

            canvasUI.SetActive(true);
            panelUI.SetActive(true);

            inspectionInstance = Instantiate(currentObject, inspectionPoint.position, Quaternion.identity);
            inspectionInstance.layer = LayerMask.NameToLayer("Inspection");
        }

        if (isInspecting && inspectionInstance != null)
        {
            float inputX = Input.GetAxis("Mouse X") * rotateVelocity;
            float inputY = Input.GetAxis("Mouse Y") * rotateVelocity;

            inspectionInstance.transform.Rotate(Vector3.up, inputX, Space.World);
            inspectionInstance.transform.Rotate(Vector3.right, -inputY, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && isInspecting)
        {
            isInspecting = false;

            Destroy(inspectionInstance);

            inspectionCamera.enabled = false;
            mainCamera.enabled = true;

            canvasUI.SetActive(false);
            panelUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            IsInsideTrigger = true;
            if (!hasKey)
            {
                Debug.Log("You need the key");
                DoorIconLock.gameObject.SetActive(true);
            }
            else if (hasKey)
            {
                DoorIconUnlock.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Door"))
        {
            IsInsideTrigger = false;
            DoorIconLock.gameObject.SetActive(false);
        }
    }
}
